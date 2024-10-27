using FinalProject3.Data;
using FinalProject3.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinalProject2.Auth
{
    public class Utils
    {


        public static void setupIdentity(WebApplicationBuilder builder)
        {

            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = false,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                MaxFailedAccessAttempts = 5
            };
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Lockout = lockoutOptions;
                }
            })
                .AddEntityFrameworkStores<FP3Context>();
        }


        public static void setupJwt(WebApplicationBuilder builder)
        {

            //fill our JwtSettings object with the appsettings.json file
            var jwtSettings = JWTSettings.NewInstance();
            builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    //bonus:
                    ValidateIssuer = true,
                    ValidateAudience = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });
        }
    }
}

