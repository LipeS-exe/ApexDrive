using ApexDrive.models;
using Microsoft.EntityFrameworkCore;

namespace ApexDrive.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Marca> Marcas { get; set; }
}
