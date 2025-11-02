using System.Text.RegularExpressions;
using static PermitManagement.Shared.Constants;

namespace PermitManagement.Shared;

public class ValidationRules
{
    public static readonly Regex RegPattern =
        new(@"^[A-Z]{1,3}\d{1,3}[A-Z]{0,3}$", RegexOptions.IgnoreCase); // loose UK-style check

    public const string InvalidRegistrationMessage =
        $"Invalid registration. Example formats: AB12CDE, {DefaultValidRegistrationNumber}.";
}
