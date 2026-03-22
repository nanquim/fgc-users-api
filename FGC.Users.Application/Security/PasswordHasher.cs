using Microsoft.AspNetCore.Identity;

namespace FGC.Users.Application.Security;

public static class PasswordHasher
{
    private static readonly PasswordHasher<string> _hasher = new();

    public static string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }
}
