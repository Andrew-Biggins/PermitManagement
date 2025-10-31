using PermitManagement.Core.Entities;

namespace PermitManagement.Core.Interfaces;

public interface IPermitService
{
    Task AddPermitAsync(Permit permit);
    Task<bool> HasValidPermitAsync(Vehicle vehicle, Zone zone, DateTime? date = null);
    Task<IEnumerable<Permit>> GetActivePermitsAsync(Zone zone, DateTime? date = null);
}