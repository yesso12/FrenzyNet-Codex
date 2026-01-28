using FrenzyNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.DeviceCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasIndex(d => new { d.UserId, d.Name }).IsUnique();
            entity.HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId);
        });
    }
}
