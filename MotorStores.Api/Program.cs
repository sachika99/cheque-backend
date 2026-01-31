using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotorStores.Application;
using MotorStores.Infrastructure;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

/* =========================
   SERVICES
========================= */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(cfg);

builder.Services.AddScoped<EmailService>();

// ✅ Read connection string from environment variable OR appsettings.json
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? cfg.GetConnectionString("DefaultConnection");

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
        // ✅ Read from environment variables first
        ValidIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? cfg["Jwt:Issuer"],
        ValidAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? cfg["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("Jwt__Key") ?? cfg["Jwt:Key"]!)
        ),
        ClockSkew = TimeSpan.Zero
    };
});

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("frontend");

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
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
```

## Key Changes:

1. * *Connection String * *: Reads from `ConnectionStrings__DefaultConnection` environment variable
2. **JWT Config**: Reads `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key` from environment variables
3. **Port**: Always binds to `0.0.0.0` with Railway's PORT

## Update Railway Variables:

In Railway, update your variable names to match .NET's format:
```
ConnectionStrings__DefaultConnection = Server=my-sqlserver-db...
Jwt__Issuer = https://your-railway-app.up.railway.app
Jwt__Audience = https://your-frontend-url.com
Jwt__Key = h3Y7e2aZ9qT1xW8pS4uN0vL6kB5rC3mD
Jwt__AccessTokenMinutes = 10
Jwt__RefreshTokenDays = 14
Email__From = no - reply@motorstores.com
Email__SmtpHost = smtp.gmail.com
Email__SmtpPort = 587
Email__User = devtesting648@gmail.com
Email__Password = lyrmfufrvclngyna