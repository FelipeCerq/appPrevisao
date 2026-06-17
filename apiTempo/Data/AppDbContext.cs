using Microsoft.EntityFrameworkCore;
using apiTempo.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CidadeFavorita> CidadesFavoritas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}