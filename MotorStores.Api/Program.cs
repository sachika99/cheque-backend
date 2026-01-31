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
    Console.WriteLine("=== Application Starting ===");

    var builder = WebApplication.CreateBuilder(args);
    var cfg = builder.Configuration;

    /* =========================
       PORT HANDLING (Railway) - MUST BE BEFORE SERVICES
    ========================= */
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"✅ Configured to listen on port: {port}");

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

    // Database Configuration with validation
    var connectionString = cfg.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("❌ Database connection string 'DefaultConnection' is missing!");
    }
    Console.WriteLine("✅ Connection string found");

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

    // JWT Configuration with validation
    var jwtKey = cfg["Jwt:Key"];
    var jwtIssuer = cfg["Jwt:Issuer"];
    var jwtAudience = cfg["Jwt:Audience"];

    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("❌ JWT Key is missing!");
    }
    if (string.IsNullOrEmpty(jwtIssuer))
    {
        throw new InvalidOperationException("❌ JWT Issuer is missing!");
    }
    if (string.IsNullOrEmpty(jwtAudience))
    {
        throw new InvalidOperationException("❌ JWT Audience is missing!");
    }

    Console.WriteLine($"✅ JWT Config - Issuer: {jwtIssuer}, Audience: {jwtAudience}");

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
    Console.WriteLine("🔨 Building application...");
    var app = builder.Build();

    /* =========================
       DATABASE MIGRATION (Optional - Uncomment if needed)
    ========================= */
    // Console.WriteLine("📊 Applying database migrations...");
    // try
    // {
    //     using (var scope = app.Services.CreateScope())
    //     {
    //         var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //         db.Database.Migrate();
    //         Console.WriteLine("✅ Database migrations applied successfully");
    //     }
    // }
    // catch (Exception ex)
    // {
    //     Console.WriteLine($"⚠️ Migration warning: {ex.Message}");
    //     // Don't crash if migrations fail - database might already be up to date
    // }

    /* =========================
       SWAGGER
    ========================= */
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("✅ Swagger configured");

    /* =========================
       MIDDLEWARE
    ========================= */
    app.UseCors("frontend");

    // HTTPS only for local development
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        Console.WriteLine("🔒 HTTPS redirection enabled (Development)");
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine("=== ✅ Application Started Successfully ===");
    Console.WriteLine($"🌐 Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($"🚀 Listening on: http://0.0.0.0:{port}");
    Console.WriteLine($"📚 Swagger UI: http://0.0.0.0:{port}/swagger");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("=== ❌ FATAL ERROR ===");
    Console.WriteLine($"Error Type: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");

    if (ex.InnerException != null)
    {
        Console.WriteLine("\n=== Inner Exception ===");
        Console.WriteLine($"Type: {ex.InnerException.GetType().Name}");
        Console.WriteLine($"Message: {ex.InnerException.Message}");
        Console.WriteLine($"Stack Trace:\n{ex.InnerException.StackTrace}");
    }

    // Re-throw to ensure the app exits with error code
    throw;
}