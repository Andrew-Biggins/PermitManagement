namespace PermitManagement.Core.Entities;

public class Permit
{
    public int Id { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public Zone Zone { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public Permit(Vehicle vehicle, Zone zone, DateTime startDate, DateTime endDate)
    {
        Vehicle = vehicle;
        Zone = zone;
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool IsActive(DateTime date) => date >= StartDate && date <= EndDate;

    // For EF Core only
    private Permit() { }
}
