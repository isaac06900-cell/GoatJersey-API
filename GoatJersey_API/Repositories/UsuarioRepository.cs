using GoatJersey_API.Data;
using GoatJersey_API.Models;
using Microsoft.EntityFrameworkCore;

namespace GoatJersey_API.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task AgregarAsync(Usuario usuario);
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        => _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

    public async Task AgregarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}
