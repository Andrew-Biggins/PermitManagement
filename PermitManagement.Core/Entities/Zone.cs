namespace PermitManagement.Core.Entities;
public record Zone
{
    public string Name { get; set; } = string.Empty;

    public Zone() { } // for JSON / EF
    public Zone(string name) => Name = name;
}