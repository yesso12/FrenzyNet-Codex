using FrenzyNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
}
