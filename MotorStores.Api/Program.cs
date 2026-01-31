using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotorStores.Application;
using MotorStores.Infrastructure;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;
using System.Text;

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

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

/* =========================
   JWT AUTH
========================= */

var jwtKey = cfg["Jwt:Key"];
var jwtIssuer = cfg["Jwt:Issuer"];
var jwtAudience = cfg["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            // ✅ keep this true in real prod if you want token expiry enforced
            ValidateLifetime = false,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey ?? "TEMP_DEV_KEY_CHANGE_ME_123456789")
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

/* =========================
   CORS
========================= */

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(origin =>
            origin.StartsWith("http://localhost") ||
            origin.StartsWith("https://localhost") ||
            origin.Contains(".railway.app")
        ));
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddSignalR();

/* =========================
   BUILD APP
========================= */

var app = builder.Build();

/* =========================
   SWAGGER
========================= */

// ✅ Usually keep Swagger only in dev (safer)
// If you want Swagger in Railway too, remove this if and keep always.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* =========================
   MIDDLEWARE
========================= */

app.UseCors("frontend");

// ✅ HTTPS redirect only for local dev
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapHub<NotificationHub>("/hubs/notification");

/* =========================
   ✅ RAILWAY PORT FIX
========================= */

// Railway provides PORT env var, and expects 0.0.0.0 binding.
var port = Environment.GetEnvironmentVariable("PORT");

// Only add if PORT exists AND urls not already set
if (!string.IsNullOrWhiteSpace(port))
{
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();
