using Microsoft.EntityFrameworkCore;
using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;

namespace PermitManagement.Infrastructure;

public class PermitRepository(PermitDbContext context) : IPermitRepository
{
    public async Task AddAsync(Permit permit)
    {
        context.Permits.Add(permit);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Permit>> GetPermitsForVehicleAsync(Vehicle vehicle)
    {
        return await context.Permits
            .Where(p => p.Vehicle.Registration == vehicle.Registration)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permit>> GetPermitsByZoneAsync(Zone zone)
    {
        return await context.Permits
            .Where(p => p.Zone.Name == zone.Name)
            .ToListAsync();
    }
}