using FinalProject3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace FinalProject3.Data
{
    public class FP3Context(DbContextOptions<FP3Context> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Post> Post { get; set; } = default!;

        public DbSet<Comment> Comment { get; set; } = default!;

        public new DbSet<AppUser> Users { get; set; } = default!;
        public DbSet<Chat> Chat { get; set; } = default!;

        public DbSet<Notification> Notification { get; set; } = default!;

        public DbSet<Message> Message { get; set; } = default!;

        public DbSet<SocialGroup> Group { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
    .HasMany(a => a.SocialGroups)
    .WithMany(s => s.Members)
    .UsingEntity<Dictionary<string, object>>(
        "AppUserSocialGroup",
        j => j.HasOne<SocialGroup>().WithMany().HasForeignKey("SocialGroupId"),
        j => j.HasOne<AppUser>().WithMany().HasForeignKey("AppUserId"));
            builder.Entity<IdentityUserRole<string>>()
             .HasOne<IdentityUser>()
              .WithMany()
             .HasForeignKey(ur => ur.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SocialGroup>()
    .HasOne(s => s.groupAdmin)
    .WithMany()  
    .HasForeignKey(s => s.AdminId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SocialGroup>()
.HasOne(s => s.GroupCreator)
.WithMany()
.HasForeignKey(s => s.GroupCreatorId)
.OnDelete(DeleteBehavior.Restrict);

            builder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure the following relationship
            builder.Entity<AppUser>()
                .HasMany(u => u.Following)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserFollow",
                    j => j
                        .HasOne<AppUser>()
                        .WithMany()
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Restrict), // Change to Restrict
                    j => j
                        .HasOne<AppUser>()
                        .WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade)); // Keep as Cascade

            // Configure the blocking relationship
            builder.Entity<AppUser>()
                .HasMany(u => u.Blocked)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserBlock",
                    j => j
                        .HasOne<AppUser>()
                        .WithMany()
                        .HasForeignKey("BlockedId")
                        .OnDelete(DeleteBehavior.Restrict), // Keep as Restrict
                    j => j
                        .HasOne<AppUser>()
                        .WithMany()
                        .HasForeignKey("BlockerId")
                        .OnDelete(DeleteBehavior.Cascade)); // Keep as Cascade
                                                            // Configure the Chat relationship
            builder.Entity<AppUser>()
                .HasMany(u => u.Chats) 
                .WithMany(c => c.Users) 
                .UsingEntity<Dictionary<string, object>>( 
                    "UserChats", 
                    j => j
                        .HasOne<Chat>() 
                        .WithMany() 
                        .HasForeignKey("ChatId") 
                        .OnDelete(DeleteBehavior.Restrict), 
                    j => j
                        .HasOne<AppUser>() 
                        .WithMany()
                        .HasForeignKey("UserId") 
                        .OnDelete(DeleteBehavior.Cascade) 
                );





            builder.Entity<IdentityRole>().HasData(
    new IdentityRole() { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString() }
    );
            var hasher = new PasswordHasher<AppUser>();
            var user = new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "TomerLiveChen@gmail.com",
                NormalizedEmail = "TOMERLIVECHEN@GMAIL.COM",
                UserName = "SysAdmin",
                NormalizedUserName = "SYSADMIN",
                Prefix = "Dr",
                First_Name = "Tomer",
                Last_Name = "Chen",
                Pronouns = "They",
                ImageURL = "https://i.imgur.com/1nKIWjB.gif",
                PermissionLevel = "Admin",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = hasher.HashPassword(null, "qwertyU1!"),

            };
            builder.Entity<AppUser>().HasData(user);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { RoleId = "1", UserId = user.Id });


           // SeedData.SeedUsersAndPosts(builder);
        }


    }
}
