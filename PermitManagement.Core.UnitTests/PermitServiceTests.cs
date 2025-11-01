using NSubstitute;
using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;
using PermitManagement.Core.Services;
using PermitManagement.Testing.Shared;

namespace PermitManagement.Core.UnitTests;

public class PermitServiceTests
{
    private readonly IPermitRepository _repo = Substitute.For<IPermitRepository>();
    private readonly IDateTimeProvider _clock = Substitute.For<IDateTimeProvider>();
    private readonly PermitService _service;
    private readonly DateTime _fixedNow = new(2025, 10, 15);
    private readonly Vehicle _testVehicle = new("TEST123");
    private readonly Zone _testZoneA = new("A");
    private readonly Zone _testZoneB = new("B");

    public PermitServiceTests() => _service = new PermitService(_repo, _clock);

    [Gwt("Given a vehicle with an active permit in the same zone",
        "when checking if that vehicle has a valid permit",
        "then the result is true")]
    public async Task T0()
    {
        // Arrange
        _clock.UtcNow.Returns(_fixedNow);
        var permit = new Permit(_testVehicle, _testZoneA, _fixedNow.AddDays(-1), _fixedNow.AddDays(2));
        _repo.GetPermitsForVehicleAsync(_testVehicle).Returns([permit]);

        // Act
        var result = await _service.HasValidPermitAsync(_testVehicle,_testZoneA);

        // Assert
        Assert.True(result);
    }

    [Gwt("Given a new permit with a valid date range",
    "when adding it via the permit service",
    "then the repository AddAsync method is called once")]
    public async Task T1()
    {
        // Arrange
        var permit = new Permit(_testVehicle, _testZoneA, _fixedNow, _fixedNow.AddDays(1));

        // Act
        await _service.AddPermitAsync(permit);

        // Assert
        await _repo.Received(1).AddAsync(permit);
    }

    [Gwt("Given a permit where the start date is after the end date",
        "when adding it via the permit service",
        "then an ArgumentException is thrown")]
    public async Task T2()
    {
        // Arrange
        var permit = new Permit(_testVehicle, _testZoneA, _fixedNow.AddDays(1), _fixedNow);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddPermitAsync(permit));
    }

    [Gwt("Given a vehicle with one active permit for a zone",
        "when checking validity for that zone",
        "then the result is true")]
    public async Task T3()
    {
        // Arrange
        _clock.UtcNow.Returns(_fixedNow);
        var permit = new Permit(_testVehicle, _testZoneA, _fixedNow, _fixedNow.AddDays(1));
        _repo.GetPermitsForVehicleAsync(Arg.Any<Vehicle>()).Returns([permit]);

        // Act
        var result = await _service.HasValidPermitAsync(_testVehicle, _testZoneA);

        // Assert
        Assert.True(result);
    }

    [Gwt("Given a vehicle with a permit for another zone",
        "when checking validity for a different zone",
        "then the result is false")]
    public async Task T4()
    {
        // Arrange
        _clock.UtcNow.Returns(_fixedNow);
        var permit = new Permit(_testVehicle, _testZoneA, _fixedNow.AddDays(-1), _fixedNow.AddDays(5));
        _repo.GetPermitsForVehicleAsync(Arg.Any<Vehicle>()).Returns([permit]);

        // Act
        var result = await _service.HasValidPermitAsync(_testVehicle, _testZoneB);

        // Assert
        Assert.False(result);
    }

    [Gwt("Given a vehicle with no permits",
        "when checking validity for any zone",
        "then the result is false")]
    public async Task T5()
    {
        // Arrange
        _clock.UtcNow.Returns(DateTime.UtcNow);
        _repo.GetPermitsForVehicleAsync(Arg.Any<Vehicle>()).Returns([]);

        // Act
        var result = await _service.HasValidPermitAsync(_testVehicle, _testZoneA);

        // Assert
        Assert.False(result);
    }

    [Gwt("Given a zone with multiple permits where only some are active",
        "when getting active permits for that zone",
        "then only active ones are returned")]
    public async Task T6()
    {
        // Arrange
        _clock.UtcNow.Returns(_fixedNow);

        var active = new Permit(_testVehicle, _testZoneA,
            _fixedNow.AddDays(-1), _fixedNow.AddDays(5));
        
        var expired = new Permit(_testVehicle, _testZoneA,
            _fixedNow.AddDays(-10), _fixedNow.AddDays(-5));

        _repo.GetPermitsByZoneAsync(_testZoneA).Returns([active, expired]);

        // Act
        var result = await _service.GetActivePermitsAsync(_testZoneA);

        // Assert
        Assert.Collection(result, r => Assert.Equal(_testVehicle, _testVehicle));
    }
}