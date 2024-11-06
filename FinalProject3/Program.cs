
using FinalProject2.Auth;
using FinalProject3.Auth;
using FinalProject3.Data;
using FinalProject3.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinalProject3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FP3Context>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("FP3Context") ?? throw new InvalidOperationException("Connection string 'FP3Context' not found.")));
            Utils.setupIdentity(builder);
            Utils.setupJwt(builder);



            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var corsPolicy = "CorsPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicy, policy =>
                {
                    policy.WithOrigins([
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "https://white-pond-0c2316100.5.azurestaticapps.net",
                        "https://final-project-front-nu.vercel.app","https://finalprojectfront-6dxw.onrender.com"
                        //host
                        ]).AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithExposedHeaders("Authorization");

                });
            });
            var app = builder.Build();
            app.UseRouting();
            app.UseCors(corsPolicy);

            // Configure the HTTP request pipeline.
          //  if (app.Environment.IsDevelopment())
          //  {
                app.UseSwagger();
                app.UseSwaggerUI();
            //  }

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<FP3Context>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Migration error" + ex.Message);
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
