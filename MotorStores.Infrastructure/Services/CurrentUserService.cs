// Infrastructure/Services/CurrentUserService.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MotorStores.Application.Interfaces;

namespace MotorStores.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string UserId
    {
        get
        {
            var role = User?.FindFirstValue("role");
            var isAdmin = role == "admin";

            if (isAdmin)
            {
                return User?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("User not logged in.");
            }
            else
            {
                return User?.FindFirstValue("createdBy")
                    ?? throw new UnauthorizedAccessException("User not logged in.");
            }
        }
    }

    public string? UserName =>
        User?.FindFirstValue(ClaimTypes.Name);

    public string? CreatedBy =>
        User?.FindFirstValue("createdBy");

    public string? Role =>
        User?.FindFirstValue("role");
}