using PermitManagement.Core.Entities;

namespace PermitManagement.Core.Interfaces;
public interface IPermitRepository
{
    Task AddAsync(Permit permit);
    Task<IEnumerable<Permit>> GetPermitsForVehicleAsync(Vehicle vehicle);
    Task<IEnumerable<Permit>> GetPermitsByZoneAsync(Zone zone);
    Task<IEnumerable<Permit>> GetAllPermitsAsync();
}