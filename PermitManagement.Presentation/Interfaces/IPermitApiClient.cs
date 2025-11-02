using PermitManagement.Core.Entities;

namespace PermitManagement.Presentation.Interfaces;
public interface IPermitApiClient
{
    Task<IEnumerable<Permit>> GetActivePermitsAsync(string? zone = null, DateTime? date = null);
    Task AddPermitAsync(Permit permit);
    Task<bool> CheckPermitAsync(string reg, string zone);
}