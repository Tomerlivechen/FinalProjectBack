using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject3.Data
{
    public class SeedData
    {
        public static void SeedUsersAndPosts(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<AppUser>();
            var random = new Random();

            // List to store users
            var users = new List<AppUser>();

            // Create 20 users
            for (int i = 2; i <= 21; i++)
            {
                var user = new AppUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = $"User{i}@example.com",
                    NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                    UserName = $"User{i}",
                    NormalizedUserName = $"USER{i}",
                    Prefix = "Mr",
                    First_Name = $"FirstName{i}",
                    Last_Name = $"LastName{i}",
                    Pronouns = "They/Them",
                    ImageURL = $"https://i.imgur.com/1nKIWjB.gif",
                    PermissionLevel = "User",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Password!123"),
                };

                users.Add(user);
                builder.Entity<AppUser>().HasData(user);

                // Assign "User" role to each user
                builder.Entity<IdentityUserRole<string>>().HasData(
                    new IdentityUserRole<string>
                    {
                        RoleId = "3",  // Assuming Role ID "1" is for the "User" role
                        UserId = user.Id
                    });
            }

        }
    }
}
