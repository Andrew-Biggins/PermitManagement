namespace PermitManagement.Core.Entities;

public class Permit
{
    public int Id { get; set; }
    public Vehicle Vehicle { get; set; } = new();
    public Zone Zone { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Permit() { }

    public Permit(Vehicle vehicle, Zone zone, DateTime startDate, DateTime endDate)
    {
        Vehicle = vehicle;
        Zone = zone;
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool IsActive(DateTime date) => date >= StartDate && date <= EndDate;
}