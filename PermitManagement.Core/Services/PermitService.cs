using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;

namespace PermitManagement.Core.Services;

public class PermitService(IPermitRepository repo, IDateTimeProvider clock) : IPermitService
{
    private readonly IPermitRepository _repo = repo;
    private readonly IDateTimeProvider _clock = clock;

    public async Task AddPermitAsync(Permit permit)
    {
        if (permit.StartDate > permit.EndDate)
            throw new ArgumentException("Start date must be before end date.");

        await _repo.AddAsync(permit);
    }

    public async Task<bool> HasValidPermitAsync(Vehicle vehicle, Zone zone, DateTime? date = null)
    {
        var now = date ?? _clock.UtcNow;
        var permits = await _repo.GetPermitsForVehicleAsync(vehicle);

        return permits.Any(p => p.Zone == zone && p.IsActive(now));
    }

    public async Task<IEnumerable<Permit>> GetActivePermitsAsync(Zone zone, DateTime? date = null)
    {
        var now = date ?? _clock.UtcNow;
        var permits = await _repo.GetPermitsByZoneAsync(zone);
        return permits.Where(p => p.IsActive(now));
    }
}