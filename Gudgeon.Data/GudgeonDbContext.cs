using Microsoft.EntityFrameworkCore;
using Gudgeon.Data.Models;

namespace Gudgeon.Data;

/// <summary>
/// Implementation of <see cref="DbContext"/> for Gudgeon.
/// </summary>
public class GudgeonDbContext : DbContext
{
    public GudgeonDbContext(DbContextOptions<GudgeonDbContext> options)
        : base(options)
    {
    }

    public DbSet<Autorole> Autoroles { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Autorole>()
            .HasIndex(x => new { x.GuildId, x.UsersType })
            .IsUnique();
    }
}