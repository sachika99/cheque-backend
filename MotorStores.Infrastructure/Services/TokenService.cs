using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MotorStores.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotorStores.Infrastructure.Services
{
    public class TokenService
    {
        private readonly IConfiguration _cfg;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration cfg, UserManager<AppUser> um)
        {
            _cfg = cfg;
            _userManager = um;
        }

        public async Task<string> CreateAccessTokenAsync(AppUser user)
        {
       
            var roles = await _userManager.GetRolesAsync(user);

           
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      
            var jwt = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                signingCredentials: creds
            );

        
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
