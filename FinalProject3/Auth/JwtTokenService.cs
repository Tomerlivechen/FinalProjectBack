using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject3.Auth
{
    public class JwtTokenService(IOptions<JWTSettings> options , UserManager<AppUser> userManager) : IJwtTokenService
    {
        JWTSettings jwtSettings = options.Value;

        public async Task<string> CreateToken(AppUser user)
        {

            if (user is null || user.UserName is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName),
                new Claim("PermissionLevel", user.PermissionLevel)
            ];
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                //Custom claim
                claims.Add(new Claim("IsAdmin", "true"));
                //built-in claim
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim("IsAdmin", "false"));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            //our secret key as bytes:
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey ?? throw new InvalidOperationException("SecretKey is missing.")));

            //SingningCredentials = key + algorithm:
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken( token);
        }
    }
}
