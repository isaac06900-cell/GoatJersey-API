using GoatJersey_API.Data;
using GoatJersey_API.DTOs;
using GoatJersey_API.Models;
using Microsoft.EntityFrameworkCore;

namespace GoatJersey_API.Repositories;

public interface IPublicacionRepository
{
    Task<List<Publicacion>> ObtenerActivasDeOtrosAsync(int idUsuario, PublicacionFiltrosDto filtros);
    Task<Publicacion?> ObtenerDetalleAsync(int idPublicacion);
    Task<Publicacion?> ObtenerPorIdAsync(int idPublicacion);
    Task<bool> TieneActivaAsync(int idJersey);
    Task AgregarAsync(Publicacion publicacion);
    Task GuardarCambiosAsync();
}

public class PublicacionRepository : IPublicacionRepository
{
    private readonly ApplicationDbContext _context;

    public PublicacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Publicacion>> ObtenerActivasDeOtrosAsync(int idUsuario, PublicacionFiltrosDto f)
    {
        var query = _context.Publicaciones
            .AsNoTracking()
            .Include(p => p.Jersey!).ThenInclude(j => j.Usuario)
            .Where(p => p.Estatus == "Activa" && p.Jersey!.IdUsuario != idUsuario);

        if (!string.IsNullOrWhiteSpace(f.Equipo))
        {
            var equipo = f.Equipo.Trim();
            query = query.Where(p => p.Jersey!.Equipo.Contains(equipo));
        }
        if (!string.IsNullOrWhiteSpace(f.Temporada))
            query = query.Where(p => p.Jersey!.Temporada == f.Temporada.Trim());
        if (!string.IsNullOrWhiteSpace(f.Talla))
            query = query.Where(p => p.Jersey!.Talla == f.Talla.Trim());
        if (!string.IsNullOrWhiteSpace(f.Version))
            query = query.Where(p => p.Jersey!.Version == f.Version.Trim());

        // Filtrar por Venta también muestra las publicaciones "Ambos", y lo mismo con Intercambio.
        if (!string.IsNullOrWhiteSpace(f.Tipo))
        {
            var tipo = f.Tipo.Trim();
            query = query.Where(p => p.Tipo == tipo || p.Tipo == "Ambos");
        }

        return await query.OrderByDescending(p => p.FechaPublicacion).ToListAsync();
    }

    public Task<Publicacion?> ObtenerDetalleAsync(int idPublicacion)
        => _context.Publicaciones
            .AsNoTracking()
            .Include(p => p.Jersey!).ThenInclude(j => j.Usuario)
            .FirstOrDefaultAsync(p => p.IdPublicacion == idPublicacion);

    // Con seguimiento y con el jersey cargado: se usa para retirar.
    public Task<Publicacion?> ObtenerPorIdAsync(int idPublicacion)
        => _context.Publicaciones
            .Include(p => p.Jersey)
            .FirstOrDefaultAsync(p => p.IdPublicacion == idPublicacion);

    public Task<bool> TieneActivaAsync(int idJersey)
        => _context.Publicaciones.AnyAsync(p => p.IdJersey == idJersey && p.Estatus == "Activa");

    public Task AgregarAsync(Publicacion publicacion)
    {
        _context.Publicaciones.Add(publicacion);
        return Task.CompletedTask;
    }

    // Un solo SaveChanges guarda juntos la publicación y el cambio de Disponibilidad del jersey.
    public Task GuardarCambiosAsync() => _context.SaveChangesAsync();
}
