using GoatJersey_API.Models;
using Microsoft.EntityFrameworkCore;

namespace GoatJersey_API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jersey> Jerseys => Set<Jersey>();
    public DbSet<Publicacion> Publicaciones => Set<Publicacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.IdUsuario);
            e.Property(x => x.Nombre).HasMaxLength(100);
            e.Property(x => x.Correo).HasMaxLength(150);
            e.Property(x => x.PasswordHash).HasMaxLength(200);
            e.Property(x => x.Rol).HasMaxLength(20);
            e.HasIndex(x => x.Correo).IsUnique();
        });

        modelBuilder.Entity<Jersey>(e =>
        {
            e.ToTable("Jerseys");
            e.HasKey(x => x.IdJersey);
            e.Property(x => x.ValorEstimado).HasPrecision(10, 2);
            e.HasOne(x => x.Usuario)
             .WithMany(u => u.Jerseys)
             .HasForeignKey(x => x.IdUsuario)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Publicacion>(e =>
        {
            e.ToTable("Publicaciones");
            e.HasKey(x => x.IdPublicacion);
            e.Property(x => x.Precio).HasPrecision(10, 2);
            e.HasOne(x => x.Jersey)
             .WithMany(j => j.Publicaciones)
             .HasForeignKey(x => x.IdJersey)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
