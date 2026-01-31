using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MotorStores.Application;
using MotorStores.Infrastructure;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var cfg = builder.Configuration;

    /* =========================
       CONFIGURATION LOGGING
    ========================= */
    Console.WriteLine("=== Configuration Check ===");
    Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

    // Read from environment variables first, fallback to appsettings.json
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                           ?? cfg.GetConnectionString("DefaultConnection");
    var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? cfg["Jwt:Key"];
    var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? cfg["Jwt:Issuer"];
    var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? cfg["Jwt:Audience"];
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

    Console.WriteLine($"✅ Connection String: {(!string.IsNullOrEmpty(connectionString) ? "SET" : "MISSING")}");
    Console.WriteLine($"✅ JWT Key: {(!string.IsNullOrEmpty(jwtKey) ? "SET" : "MISSING")}");
    Console.WriteLine($"✅ JWT Issuer: {jwtIssuer ?? "MISSING"}");
    Console.WriteLine($"✅ JWT Audience: {jwtAudience ?? "MISSING"}");
    Console.WriteLine($"✅ Port: {port}");
    Console.WriteLine("===========================");

    // Validate critical configuration
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("❌ Connection string is missing!");
    if (string.IsNullOrEmpty(jwtKey))
        throw new InvalidOperationException("❌ JWT Key is missing!");

    /* =========================
       SERVICES
    ========================= */
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(cfg);

    // ✅ REQUIRED – AuthController depends on this
    builder.Services.AddScoped<EmailService>();

    // ✅ Use the resolved connection string
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(connectionString));

    builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 8;
        opt.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

    // ✅ Railway-safe CORS (allow ANY frontend)
    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("frontend", p => p
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
    });

    builder.Services.AddScoped<TokenService>();
    builder.Services.AddSignalR();

    /* =========================
       BUILD
    ========================= */
    var app = builder.Build();

    /* =========================
       DATABASE CONNECTION TEST
    ========================= */
    try
    {
        Console.WriteLine("Testing database connection...");
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var canConnect = await db.Database.CanConnectAsync();
            if (canConnect)
            {
                Console.WriteLine("✅ Database connected successfully");
            }
            else
            {
                Console.WriteLine("⚠️ Database connection test returned false");
            }
        }
    }
    catch (Exception dbEx)
    {
        Console.WriteLine($"⚠️ Database connection warning: {dbEx.Message}");
        Console.WriteLine("Application will continue, but database operations may fail.");
    }

    /* =========================
       SWAGGER
    ========================= */
    app.UseSwagger();
    app.UseSwaggerUI();

    /* =========================
       MIDDLEWARE
    ========================= */
    app.UseCors("frontend");

    // HTTPS only for local
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    /* =========================
       PORT HANDLING (Railway)
    ========================= */
    app.Urls.Clear(); // Clear default URLs
    app.Urls.Add($"http://0.0.0.0:{port}");

    Console.WriteLine($"🚀 Starting server on http://0.0.0.0:{port}");
    Console.WriteLine($"📝 Swagger available at http://0.0.0.0:{port}/swagger");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("╔════════════════════════════════════════╗");
    Console.WriteLine("║        ❌ FATAL STARTUP ERROR          ║");
    Console.WriteLine("╚════════════════════════════════════════╝");
    Console.WriteLine($"Error Type: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"\n📍 Inner Exception:");
        Console.WriteLine($"Type: {ex.InnerException.GetType().Name}");
        Console.WriteLine($"Message: {ex.InnerException.Message}");
    }

    Console.WriteLine("\n💡 Troubleshooting:");
    Console.WriteLine("1. Check Railway environment variables are set correctly");
    Console.WriteLine("2. Verify database connection string");
    Console.WriteLine("3. Ensure JWT settings are configured");
    Console.WriteLine("4. Check application logs above for configuration status");

    // Re-throw to ensure Railway sees the failure
    throw;
}