using System.Text.RegularExpressions;

namespace FGC.Users.Domain.ValueObjects;

public sealed class Email
{
    private static readonly Regex _regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; private set; }

    private Email()
    {
        Value = null!;
    }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !_regex.IsMatch(value))
            throw new ArgumentException("Email inválido");

        Value = value.ToLower();
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Email other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
}
