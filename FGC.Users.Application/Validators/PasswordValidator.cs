using System.Text.RegularExpressions;

namespace FGC.Users.Application.Validators;

public static class PasswordValidator
{
    public static bool IsValid(string password)
    {
        if (password.Length < 8) return false;
        if (!Regex.IsMatch(password, @"[A-Za-z]")) return false;
        if (!Regex.IsMatch(password, @"\d")) return false;
        if (!Regex.IsMatch(password, @"[^\w\d\s]")) return false;

        return true;
    }
}
