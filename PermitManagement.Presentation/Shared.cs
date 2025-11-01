using System.Text.RegularExpressions;

namespace PermitManagement.Presentation;
public static class Shared
{
    public static readonly Regex RegPattern =
        new(@"^[A-Z]{1,3}\d{1,3}[A-Z]{0,3}$", RegexOptions.IgnoreCase); // loose UK-style check

    public const string InvalidRegistrationMessage =
        "Invalid registration. Example formats: AB12CDE, A123BC.";
}