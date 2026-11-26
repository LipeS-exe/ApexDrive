using ApexDrive.models;
using ApexDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexDrive.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Marca> Marcas { get; set; }
    public DbSet<Opcional> Opcionais { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1:N -> uma Marca tem vários Veículos
        // Restrict: não deixa apagar uma marca que ainda tem carros (RN03)
        modelBuilder.Entity<Veiculo>()
            .HasOne(v => v.Marca)
            .WithMany(m => m.Veiculos)
            .HasForeignKey(v => v.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);

        // N:N -> um Veículo tem vários Opcionais e um Opcional está em vários Veículos
        modelBuilder.Entity<Veiculo>()
            .HasMany(v => v.Opcionais)
            .WithMany(o => o.Veiculos)
            .UsingEntity(j => j.ToTable("VeiculoOpcional"));

        // Placa única também no banco (RN01)
        modelBuilder.Entity<Veiculo>()
            .HasIndex(v => v.Placa)
            .IsUnique();
    }
}
