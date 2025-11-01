using PermitManagement.Core.Entities;
using PermitManagement.Testing.Shared;
using static PermitManagement.Testing.Shared.TestFixtures;

namespace PermitManagement.Core.UnitTests;
public class PermitTests
{
    [Gwt("Given a permit that starts before and ends after the given date",
        "when checking if the permit is active",
        "then it returns true")]
    public void T0()
    {
        // Arrange
        var permit = new Permit(VehicleABC123, ZoneA, DefaultStart, DefaultEnd);

        // Act
        var result = permit.IsActive(DefaultStart.AddDays(1));

        // Assert
        Assert.True(result);
    }

    [GwtTheory("Given a permit with a fixed start and end date",
        "when checking if the permit is active for dates outside that range",
        "then IsActive returns false")]
    [InlineData(-1)]                             // before start
    [InlineData(DefaultPermitDurationDays + 1)]  // after end
    public void T1(int elapsedDays)
    {
        // Arrange
        var permit = new Permit(VehicleABC123, ZoneA, DefaultStart, DefaultEnd);

        // Act
        var result = permit.IsActive(DefaultStart.AddDays(elapsedDays));

        // Assert
        Assert.False(result);
    }
}
