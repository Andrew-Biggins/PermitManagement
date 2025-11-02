using NSubstitute;
using PermitManagement.Core.Entities;
using PermitManagement.Presentation.Interfaces;
using PermitManagement.Testing.Shared;
using static PermitManagement.Shared.Constants;
using static PermitManagement.Shared.ValidationRules;

namespace PermitManagement.Presentation.UnitTests;

public class PermitViewModelTests
{
    [Gwt("Given a PermitViewModel",
        "when constructed",
        "then VehicleRegistration starts with the default valid reg and HasErrors is false")]
    public void T0()
    {        
        // Act
        var vm = new PermitViewModel(SubApi);

        // Assert
        Assert.Equal(DefaultValidRegistrationNumber, vm.VehicleRegistration);
        Assert.False(vm.HasErrors);
        Assert.True(vm.AddPermitCommand.CanExecute(null));
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration is valid",
        "then validation passes and AddPermitCommand is enabled")]
    public void T1()
    {
        // Arrange
        var vm = new PermitViewModel(SubApi);

        // Act
        vm.VehicleRegistration = ValidRegNumber;

        // Assert
        Assert.False(vm.HasErrors);
        Assert.True(vm.AddPermitCommand.CanExecute(null));
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration is invalid",
        "then an error is set and AddPermitCommand is disabled")]
    public void T2()
    {
        // Arrange
        var vm = new PermitViewModel(SubApi);

        // Act
        vm.VehicleRegistration = InvalidRegNumber;

        // Assert
        Assert.True(vm.HasErrors);
        Assert.False(vm.AddPermitCommand.CanExecute(null));

        var errors = vm.GetErrors(nameof(vm.VehicleRegistration)).OfType<string>().ToList();
        Assert.Contains(InvalidRegistrationMessage, errors);
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration changes",
        "then PropertyChanged is raised for VehicleRegistration")]
    public void T3()
    {
        // Arrange
        var vm = new PermitViewModel(SubApi);
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        // Act
        vm.VehicleRegistration = ValidRegNumber;

        // Assert
        Assert.Contains(nameof(vm.VehicleRegistration), raised);
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration is set to the same value again",
        "then PropertyChanged is not raised")]
    public void T4()
    {
        // Arrange
        var vm = new PermitViewModel(SubApi) { VehicleRegistration = ValidRegNumber };
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        // Act
        vm.VehicleRegistration = ValidRegNumber;

        // Assert
        Assert.DoesNotContain(nameof(vm.VehicleRegistration), raised);
    }

    [Gwt("Given a PermitViewModel with valid registration",
        "when AddPermitAsync is executed",
        "then AddPermitAsync and GetActivePermitsAsync are called on the API")]
    public async Task T5()
    {
        // Arrange
        var api = SubApi;
        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        api.ClearReceivedCalls();
        api.GetActivePermitsAsync("A").Returns([]);

        // Act
        await vm.AddPermitAsync();

        // Assert
        await api.Received(1)
                 .AddPermitAsync(Arg.Is<Permit>(p =>
                        p.Vehicle.Registration == ValidRegNumber &&
                        p.Zone.Name == "A"));

        await api.Received(1).GetActivePermitsAsync("A");
    }

    [Gwt("Given a PermitViewModel with existing permits",
        "when LoadPermitsAsync is executed",
        "then Permits collection is populated")]
    public async Task T6()
    {
        // Arrange
        var api = Substitute.For<IPermitApiClient>();
        var permits = new[]
        {
            new Permit(new Vehicle("CAR001"), new Zone("A"), DateTime.Today, DateTime.Today.AddDays(7))
        };
        api.GetActivePermitsAsync("A").Returns(permits);

        var vm = new PermitViewModel(api);

        // Act
        await vm.LoadPermitsAsync();

        // Assert
        Assert.Single(vm.Permits);
        Assert.Equal("CAR001", vm.Permits.First().Vehicle.Registration);
    }

    [Gwt("Given a PermitViewModel with valid registration",
        "when AddPermitAsync is called twice with same reg and zone",
        "then API AddPermitAsync is called twice and LoadPermitsAsync each time")]
    public async Task T7()
    {
        // Arrange
        var api = SubApi;
        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "B"
        };

        api.ClearReceivedCalls();

        api.GetActivePermitsAsync("B").Returns([]);

        // Act
        await vm.AddPermitAsync();
        await vm.AddPermitAsync();

        // Assert
        await api.Received(2).AddPermitAsync(Arg.Any<Permit>());
        await api.Received(2).GetActivePermitsAsync("B");
    }

    [Gwt("Given a PermitViewModel",
        "when SelectedZone is changed",
        "then PropertyChanged is raised for SelectedZone")]
    public void T8()
    {
        // Arrange
        var vm = new PermitViewModel(SubApi);
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        // Act
        vm.SelectedZone = "B";

        // Assert
        Assert.Contains(nameof(vm.SelectedZone), raised);
    }

    [Gwt("Given a PermitViewModel with ShowAllPermits off",
        "when ShowAllPermits is toggled on",
        "then GetActivePermitsAsync is called with null")]
    public async Task T10()
    {
        // Arrange
        var api = SubApi;
        api.GetActivePermitsAsync(null).Returns([]);
        var vm = new PermitViewModel(api);
        api.ClearReceivedCalls();

        // Act
        vm.ShowAllPermits = true;
        await Task.Delay(50); // give async call time

        // Assert
        await api.Received(1).GetActivePermitsAsync(null);
    }

    [Gwt("Given a PermitViewModel with ShowAllPermits on",
        "when ShowAllPermits is toggled off",
        "then GetActivePermitsAsync is called with the zone arg")]
    public async Task T11()
    {
        // Arrange
        var api = SubApi;
        api.GetActivePermitsAsync(null).Returns([]);

        var vm = new PermitViewModel(api);
        vm.ShowAllPermits = true;
        api.ClearReceivedCalls();

        // Act
        vm.ShowAllPermits = false;
        await Task.Delay(50); // give async call time

        // Assert
        await api.Received(1).GetActivePermitsAsync(vm.SelectedZone);
    }

    [Gwt("Given a PermitViewModel",
        "when SelectedZone is changed",
        "then CheckPermitAsync and GetActivePermitsAsync for that zone are called")]
    public async Task T12()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        api.GetActivePermitsAsync("B").Returns([]);

        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber
        };

        api.ClearReceivedCalls();

        // Act
        vm.SelectedZone = "B";
        await Task.Delay(50);

        // Assert
        await api.Received(1).CheckPermitAsync(ValidRegNumber, "B");
        await api.Received(1).GetActivePermitsAsync("B");
    }

    [Gwt("Given a PermitViewModel with CheckPermitAsync returning true",
        "when validity properties are read",
        "then HasValidPermit is true and NoValidPermit is false")]
    public async Task T13()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A").Returns(true);

        // Act
        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        await Task.Delay(50);

        // Act & Assert
        Assert.True(vm.HasValidPermit);
        Assert.False(vm.NoValidPermit);
    }

    [Gwt("Given a PermitViewModel with CheckPermitAsync returning false",
        "when validity properties are read",
        "then HasValidPermit is false and NoValidPermit is true")]
    public async Task T14()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A").Returns(false);

        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        await Task.Delay(50);

        // Act & Assert
        Assert.False(vm.HasValidPermit);
        Assert.True(vm.NoValidPermit);
    }

    [Gwt("Given a PermitViewModel",
        "when AddPermitAsync completes",
        "then StatusMessage is set then cleared after delay")]
    public async Task T15()
    {
        // Arrange
        var api = SubApi;
        api.GetActivePermitsAsync("A").Returns([]);
        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        // Act
        await vm.AddPermitAsync();

        // Assert
        Assert.Equal("Permit added successfully!", vm.StatusMessage);

        // allow the delay to complete
        await Task.Delay(3500);

        Assert.Equal(string.Empty, vm.StatusMessage);
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration becomes valid",
        "then CheckPermitAsync is called")]
    public async Task T16()
    {
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A").Returns(true);
        var vm = new PermitViewModel(api) { VehicleRegistration = InvalidRegNumber };

        api.ClearReceivedCalls();

        // Act
        vm.VehicleRegistration = ValidRegNumber;
        await Task.Delay(50);

        // Assert
        await api.Received(1).CheckPermitAsync(ValidRegNumber, "A");
    }

    [Gwt("Given a PermitViewModel",
        "when VehicleRegistration is invalid",
        "then CheckPermitAsync is not called")]
    public async Task T17()
    {
        // Arrange
        var api = SubApi;
        var vm = new PermitViewModel(api);

        api.ClearReceivedCalls();

        // Act
        vm.VehicleRegistration = InvalidRegNumber;
        await Task.Delay(50);

        // Assert
        await api.DidNotReceive().CheckPermitAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Gwt("Given a PermitViewModel",
        "when CheckPermitAsync result changes from false to true",
        "then HasValidPermit and NoValidPermit update correctly")]
    // checks _permitCheckResult correctly updates validity properties
    public async Task T18()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A")
            .Returns(false, true);
        api.GetActivePermitsAsync("A").Returns([]);

        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        // first check
        await Task.Delay(50);
        Assert.False(vm.HasValidPermit);
        Assert.True(vm.NoValidPermit);

        // Act - trigger second check
        await vm.AddPermitAsync();

        // Assert
        Assert.True(vm.HasValidPermit);
        Assert.False(vm.NoValidPermit);
    }

    [Gwt("Given a PermitViewModel with an initially valid registration",
        "when the registration becomes invalid",
        "then HasValidPermit and NoValidPermit both become false")]
    public async Task T19()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A").Returns(true);

        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = ValidRegNumber,
            SelectedZone = "A"
        };

        // allow initial check to run
        await Task.Delay(50);
        Assert.True(vm.HasValidPermit);

        // Act
        vm.VehicleRegistration = InvalidRegNumber; 

        // Assert
        Assert.False(vm.HasValidPermit);
        Assert.False(vm.NoValidPermit); 
    }

    [Gwt("Given a PermitViewModel that had an invalid registration",
    "when the registration becomes valid again",
    "then HasValidPermit reflects the last API result")]
    public async Task T20()
    {
        // Arrange
        var api = SubApi;
        api.CheckPermitAsync(ValidRegNumber, "A").Returns(true);
        var vm = new PermitViewModel(api)
        {
            VehicleRegistration = InvalidRegNumber,
            SelectedZone = "A"
        };

        // Act
        vm.VehicleRegistration = ValidRegNumber;
        await Task.Delay(50);

        // Assert
        Assert.True(vm.HasValidPermit);
        Assert.False(vm.NoValidPermit);
    }

    [Gwt("Given a PermitViewModel with no API client",
        "when constructed with null",
        "then an ArgumentNullException is thrown")]
    public void T21()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new PermitViewModel(null!));
        Assert.Equal("api", ex.ParamName);
    }

    private static IPermitApiClient SubApi => Substitute.For<IPermitApiClient>();

    private const string ValidRegNumber = "AB12CDE";
    private const string InvalidRegNumber = "Invalid Reg";
}
