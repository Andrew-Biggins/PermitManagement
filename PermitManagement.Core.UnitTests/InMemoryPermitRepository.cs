using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;

namespace PermitManagement.Core.UnitTests;

internal class InMemoryPermitRepository : IPermitRepository
{
    private readonly List<Permit> _permits = [];

    public Task AddAsync(Permit permit)
    {
        _permits.Add(permit);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Permit>> GetPermitsForVehicleAsync(Vehicle vehicle)
    {
        var results = _permits
            .Where(p => p.Vehicle == vehicle)
            .ToList()
            .AsEnumerable();

        return Task.FromResult(results);
    }

    public Task<IEnumerable<Permit>> GetPermitsByZoneAsync(Zone zone)
    {
        var results = _permits
            .Where(p => p.Zone == zone)
            .ToList()
            .AsEnumerable();

        return Task.FromResult(results);
    }
}