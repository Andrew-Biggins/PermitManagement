namespace PermitManagement.Core.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}