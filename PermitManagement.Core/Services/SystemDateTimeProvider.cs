using PermitManagement.Core.Interfaces;

namespace PermitManagement.Core.Services;
internal class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
