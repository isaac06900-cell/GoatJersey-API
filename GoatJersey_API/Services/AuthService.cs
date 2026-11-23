using GoatJersey_API.DTOs;
using GoatJersey_API.Helpers;
using GoatJersey_API.Models;
using GoatJersey_API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoatJersey_API.Services;

public interface IAuthService
{
    Task<string> RegistrarAsync(RegistroDto dto);
    Task<string> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly IConfiguration _config;

    public AuthService(IUsuarioRepository repository, IConfiguration config)
    {
        _repository = repository;
        _config = config;
    }

    public async Task<string> RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();

        var existe = await _repository.ObtenerPorCorreoAsync(correo);
        if (existe != null)
            throw new BusinessRuleException("El correo ya está registrado.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = "Coleccionista",          // el rol lo asigna el servidor, nunca el cliente
            FechaRegistro = DateTime.Now
        };

        await _repository.AgregarAsync(usuario);
        return "Usuario registrado exitosamente.";
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();
        var usuario = await _repository.ObtenerPorCorreoAsync(correo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new AppUnauthorizedException("Credenciales inválidas.");

        return GenerarToken(usuario);
    }

    private string GenerarToken(Usuario usuario)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Secret"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
