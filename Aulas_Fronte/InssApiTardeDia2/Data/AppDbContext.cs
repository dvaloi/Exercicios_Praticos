using InssApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Contribuinte> Contribuintes => Set<Contribuinte>();
    public DbSet<PedidoBeneficio> Pedidos => Set<PedidoBeneficio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contribuinte>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            e.Property(c => c.Nuit).HasMaxLength(9).IsRequired();
            e.HasIndex(c => c.Nuit).IsUnique();
            e.HasMany(c => c.Pedidos)
                .WithOne(p => p.Contribuinte)
                .HasForeignKey(p => p.ContribuinteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PedidoBeneficio>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Tipo).HasMaxLength(40).IsRequired();
            e.Property(p => p.Status).HasMaxLength(40).IsRequired();
        });
    }
}
