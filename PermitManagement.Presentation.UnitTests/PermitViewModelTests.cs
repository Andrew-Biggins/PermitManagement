using NSubstitute;
using PermitManagement.Core.Entities;
using PermitManagement.Presentation.Interfaces;
using PermitManagement.Testing.Shared;
using static PermitManagement.Presentation.Shared;

namespace PermitManagement.Presentation.UnitTests;

public class PermitViewModelTests
{
    [Gwt("Given a PermitViewModel",
        "when constructed",
        "then VehicleRegistration starts empty and HasErrors is true")]
    public void T0()
    {        
        // Act
        var vm = new PermitViewModel(SubApi);

        // Assert
        Assert.Equal(string.Empty, vm.VehicleRegistration);
        Assert.True(vm.HasErrors);
        Assert.False(vm.AddPermitCommand.CanExecute(null));
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
        vm.VehicleRegistration = "???";

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

        SubApi.GetActivePermitsAsync("A").Returns([]);

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

    [Gwt("Given a PermitViewModel with no API client",
        "when constructed with null",
        "then an ArgumentNullException is thrown")]
    public void T9()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new PermitViewModel(null!));
        Assert.Equal("api", ex.ParamName);
    }

    private static IPermitApiClient SubApi => Substitute.For<IPermitApiClient>();

    private const string ValidRegNumber = "AB12CDE";
}
