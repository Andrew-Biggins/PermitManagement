using PermitManagement.Core.Entities;

namespace PermitManagement.Core.UnitTests;
public class PermitTests
{
    [Gwt("Given a permit that starts before and ends after the given date",
        "when checking if the permit is active",
        "then it returns true")]
    public void T0()
    {
        // Arrange
        var permit = new Permit(
            new Vehicle("ABC123"),
            new Zone("ZoneA"),
            new DateTime(2025, 10, 1),
            new DateTime(2025, 10, 31));

        // Act
        var result = permit.IsActive(new DateTime(2025, 10, 15));

        // Assert
        Assert.True(result);
    }

    [GwtTheory("Given a permit with a fixed start and end date",
        "when checking if the permit is active for dates outside that range",
        "then IsActive returns false")]
    [InlineData("2025-09-30")]  // before start
    [InlineData("2025-11-01")]  // after end
    public void IsActive_ShouldReturnFalse_WhenDateOutsideRange(string testDate)
    {
        // Arrange
        var permit = new Permit(
            new Vehicle("ABC123"),
            new Zone("ZoneA"),
            new DateTime(2025, 10, 1),
            new DateTime(2025, 10, 31)
        );

        var date = DateTime.Parse(testDate);

        // Act
        var result = permit.IsActive(date);

        // Assert
        Assert.False(result);
    }
}
