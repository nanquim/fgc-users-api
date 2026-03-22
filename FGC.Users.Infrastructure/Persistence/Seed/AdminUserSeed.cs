using Microsoft.AspNetCore.Identity;
using FGC.Users.Domain.Entities;
using FGC.Users.Domain.Enums;
using FGC.Users.Domain.ValueObjects;
using FGC.Users.Infrastructure.Persistence.Contexts;

namespace FGC.Users.Infrastructure.Persistence.Seed;

public static class AdminUserSeed
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Users.Any(u => u.Role == UserRole.Admin))
            return;

        var hasher = new PasswordHasher<string>();

        var admin = new User(
            "Admin",
            new Email("admin@fcg.com"),
            hasher.HashPassword(null!, "Admin@123"),
            UserRole.Admin
        );

        context.Users.Add(admin);
        context.SaveChanges();
    }
}
