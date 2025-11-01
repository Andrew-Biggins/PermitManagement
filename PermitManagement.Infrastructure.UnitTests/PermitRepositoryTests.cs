using Microsoft.EntityFrameworkCore;
using PermitManagement.Core.Entities;
using PermitManagement.Testing.Shared;
using static PermitManagement.Testing.Shared.TestFixtures;

namespace PermitManagement.Infrastructure.UnitTests;

public class PermitRepositoryTests
{
    private static PermitDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PermitDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PermitDbContext(options);
    }

    [Gwt("Given a permit",
        "when it is added to the repository",
        "then it can be retrieved by vehicle registration")]
    public async Task T0()
    {
        // Arrange
        using var context = CreateContext();
        var repo = new PermitRepository(context);
        var permit = new Permit(VehicleABC123, ZoneA, DefaultStart, DefaultEnd);

        // Act
        await repo.AddAsync(permit);
        var results = await repo.GetPermitsForVehicleAsync(VehicleABC123);

        // Assert
        var retrieved = Assert.Single(results);
        Assert.Equal(VehicleABC123.Registration, retrieved.Vehicle.Registration);
        Assert.Equal(ZoneA.Name, retrieved.Zone.Name);
    }

    [Gwt("Given multiple permits in different zones",
        "when retrieving permits by zone",
        "then only permits in that zone are returned")]
    public async Task T1()
    {
        // Arrange
        using var context = CreateContext();
        var repo = new PermitRepository(context);

        var permitA = new Permit(VehicleABC123, ZoneA, DefaultStart, DefaultEnd);
        var permitB = new Permit(VehicleDEF456, ZoneB, DefaultStart, DefaultEnd);

        await repo.AddAsync(permitA);
        await repo.AddAsync(permitB);

        // Act
        var zoneAResults = await repo.GetPermitsByZoneAsync(ZoneA);

        // Assert
        var result = Assert.Single(zoneAResults);
        Assert.Equal(VehicleABC123.Registration, result.Vehicle.Registration);
        Assert.Equal(ZoneA.Name, result.Zone.Name);
    }

    [Gwt("Given a permit with vehicle and zone value objects",
        "when persisted and reloaded",
        "then owned properties are correctly mapped")]
    public async Task T2()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<PermitDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var permit = new Permit(VehicleABC123, ZoneA, DefaultStart, DefaultEnd);

        // Act
        using (var writeContext = new PermitDbContext(options))
        {
            writeContext.Permits.Add(permit);
            await writeContext.SaveChangesAsync();
        }

        // Assert
        using var readContext = new PermitDbContext(options);
        var stored = Assert.Single(readContext.Permits.ToList());
        Assert.Equal(VehicleABC123.Registration, stored.Vehicle.Registration);
        Assert.Equal(ZoneA.Name, stored.Zone.Name);
    }
}
