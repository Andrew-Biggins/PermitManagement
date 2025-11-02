namespace PermitManagement.Shared;
public static class ZoneInfo
{
    public static bool IsValid(string name)
        => Enum.TryParse(typeof(ZoneName), name, true, out _);

    public static string RangeDescription()
    {
        var names = Enum.GetNames(typeof(ZoneName))
                        .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                        .ToArray();
        return $"{names.First()} through {names.Last()}";
    }
}