using Microsoft.EntityFrameworkCore;
using FGC.Users.Domain.Entities;
using FGC.Users.Infrastructure.Persistence.Configurations;

namespace FGC.Users.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
