using PermitManagement.Core.Interfaces;

namespace PermitManagement.Core.Services;
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}