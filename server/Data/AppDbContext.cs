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
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.AcceptedTerms).HasDefaultValue(false);
            entity.HasOne(u => u.Subscription)
                .WithOne(s => s.User)
                .HasForeignKey<Subscription>(s => s.UserId);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasIndex(d => new { d.UserId, d.Name }).IsUnique();
            entity.HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId);
            entity.HasOne(d => d.Subscription)
                .WithMany()
                .HasForeignKey(d => d.SubscriptionId);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasIndex(s => s.UserId).IsUnique();
            entity.Property(s => s.DeviceCount).HasDefaultValue(0);
            entity.Property(s => s.MaxDevices).HasDefaultValue(3);
            entity.Property(s => s.Status).HasDefaultValue("active");
        });
    }
}
