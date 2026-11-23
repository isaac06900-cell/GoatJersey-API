using GoatJersey_API.Data;
using GoatJersey_API.Models;
using Microsoft.EntityFrameworkCore;

namespace GoatJersey_API.Repositories;

public interface IJerseyRepository
{
    Task<List<Jersey>> ObtenerPorUsuarioAsync(int idUsuario);
    Task<Jersey?> ObtenerPorIdAsync(int idJersey);
    Task AgregarAsync(Jersey jersey);
    Task ActualizarAsync(Jersey jersey);
    Task EliminarAsync(Jersey jersey);
    Task<bool> TienePublicacionActivaAsync(int idJersey);
}

public class JerseyRepository : IJerseyRepository
{
    private readonly ApplicationDbContext _context;

    public JerseyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Jersey>> ObtenerPorUsuarioAsync(int idUsuario)
        => _context.Jerseys
            .AsNoTracking()
            .Where(j => j.IdUsuario == idUsuario)
            .OrderByDescending(j => j.FechaAlta)
            .ToListAsync();

    public Task<Jersey?> ObtenerPorIdAsync(int idJersey)
        => _context.Jerseys.FirstOrDefaultAsync(j => j.IdJersey == idJersey);

    public async Task AgregarAsync(Jersey jersey)
    {
        _context.Jerseys.Add(jersey);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Jersey jersey)
    {
        _context.Jerseys.Update(jersey);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Jersey jersey)
    {
        _context.Jerseys.Remove(jersey);
        await _context.SaveChangesAsync();
    }

    public Task<bool> TienePublicacionActivaAsync(int idJersey)
        => _context.Publicaciones.AnyAsync(p => p.IdJersey == idJersey && p.Estatus == "Activa");
}
