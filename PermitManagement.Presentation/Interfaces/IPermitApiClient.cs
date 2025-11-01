using PermitManagement.Core.Entities;

namespace PermitManagement.Presentation.Interfaces;
public interface IPermitApiClient
{
    Task<IEnumerable<Permit>> GetActivePermitsAsync(string zone, DateTime? date = null);
    Task AddPermitAsync(Permit permit);
}