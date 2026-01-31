using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MotorStores.Application;
using MotorStores.Infrastructure;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// ================= SERVICES =================

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

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidAudience = cfg["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// ✅ ALLOW ANY FRONTEND (local, Railway, Vercel, etc.)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddSignalR();

// ================= APP =================

var app = builder.Build();

// ✅ Swagger in ALL environments (Railway needs this)
app.UseSwagger();
app.UseSwaggerUI();

// ✅ CORS (ONLY ONCE)
app.UseCors("frontend");

// ❌ HTTPS redirection breaks Railway → keep only for local
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ================= RAILWAY PORT FIX =================
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();
