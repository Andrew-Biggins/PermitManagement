using Microsoft.EntityFrameworkCore;
using PermitManagement.Core.Entities;

namespace PermitManagement.Infrastructure;

public class PermitDbContext(DbContextOptions<PermitDbContext> options) : DbContext(options)
{
    public DbSet<Permit> Permits => Set<Permit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permit>(permit =>
        {
            permit.HasKey(p => p.Id);

            // Map value objects
            permit.OwnsOne(p => p.Vehicle, v =>
            {
                v.Property(x => x.Registration)
                 .HasColumnName("VehicleReg")
                 .IsRequired();
            });

            permit.OwnsOne(p => p.Zone, z =>
            {
                z.Property(x => x.Name)
                 .HasColumnName("ZoneName")
                 .IsRequired();
            });
        });
    }
}