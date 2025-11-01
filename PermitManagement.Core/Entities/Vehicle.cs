namespace PermitManagement.Core.Entities;

public record Vehicle
{
    public string Registration { get; set; } = string.Empty;

    public Vehicle() { } // for JSON / EF
    public Vehicle(string registration) => Registration = registration;
}