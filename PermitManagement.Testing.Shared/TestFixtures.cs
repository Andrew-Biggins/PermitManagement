using PermitManagement.Core.Entities;

namespace PermitManagement.Testing.Shared;
public static class TestFixtures
{
    public static Zone ZoneA => new("A");
    public static Zone ZoneB => new("B");
    public static Vehicle VehicleABC123 => new("ABC123");
    public static Vehicle VehicleDEF456 => new("DEF456");

    public static readonly DateTime DefaultStart = DateTime.Today;
    public static readonly DateTime DefaultEnd = DefaultStart.AddDays(DefaultPermitDurationDays);

    public const int DefaultPermitDurationDays = 7;
}