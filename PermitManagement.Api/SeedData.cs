using Microsoft.EntityFrameworkCore;
using PermitManagement.Core.Entities;
using PermitManagement.Infrastructure;

namespace PermitManagement.Api;

public static class SeedData
{
    public static void Initialize(PermitDbContext context)
    {
        if (context.Permits.Any()) return;

        context.Permits.AddRange(
            new Permit(new Vehicle("ABC123"), new Zone("A"),
                DateTime.Today.AddDays(-5), DateTime.Today.AddDays(10)),
            new Permit(new Vehicle("XYZ999"), new Zone("B"),
                DateTime.Today.AddDays(-2), DateTime.Today.AddDays(3))
        );

        context.SaveChanges();
    }
}