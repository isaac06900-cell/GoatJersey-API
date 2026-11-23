using GoatJersey_API.DTOs;
using GoatJersey_API.Helpers;
using GoatJersey_API.Models;
using GoatJersey_API.Repositories;

namespace GoatJersey_API.Services;

public interface IJerseyService
{
    Task<List<JerseyResponseDto>> ObtenerMiColeccionAsync(int idUsuario);
    Task<JerseyResponseDto> CrearAsync(int idUsuario, JerseyRequestDto dto);
    Task ActualizarAsync(int idUsuario, int idJersey, JerseyRequestDto dto);
    Task EliminarAsync(int idUsuario, int idJersey);
}

public class JerseyService : IJerseyService
{
    private readonly IJerseyRepository _repository;

    public JerseyService(IJerseyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<JerseyResponseDto>> ObtenerMiColeccionAsync(int idUsuario)
    {
        var jerseys = await _repository.ObtenerPorUsuarioAsync(idUsuario);
        return jerseys.Select(ToDto).ToList();
    }

    public async Task<JerseyResponseDto> CrearAsync(int idUsuario, JerseyRequestDto dto)
    {
        var jersey = new Jersey
        {
            IdUsuario = idUsuario,
            Disponibilidad = "EnColeccion",
            FechaAlta = DateTime.Now
        };
        AplicarDatos(jersey, dto);

        await _repository.AgregarAsync(jersey);
        return ToDto(jersey);
    }

    public async Task ActualizarAsync(int idUsuario, int idJersey, JerseyRequestDto dto)
    {
        var jersey = await ObtenerPropioAsync(idUsuario, idJersey);
        AplicarDatos(jersey, dto);
        await _repository.ActualizarAsync(jersey);
    }

    public async Task EliminarAsync(int idUsuario, int idJersey)
    {
        var jersey = await ObtenerPropioAsync(idUsuario, idJersey);

        if (await _repository.TienePublicacionActivaAsync(idJersey))
            throw new BusinessRuleException(
                "No puedes eliminar un jersey con una publicación activa. Retira la publicación primero.");

        await _repository.EliminarAsync(jersey);
    }

    // Busca el jersey y valida que exista y que sea del usuario del token.
    private async Task<Jersey> ObtenerPropioAsync(int idUsuario, int idJersey)
    {
        var jersey = await _repository.ObtenerPorIdAsync(idJersey)
                     ?? throw new NotFoundException("El jersey no existe.");

        if (jersey.IdUsuario != idUsuario)
            throw new ForbiddenException("No tienes permiso para modificar este jersey.");

        return jersey;
    }

    private static void AplicarDatos(Jersey jersey, JerseyRequestDto dto)
    {
        jersey.Equipo = dto.Equipo.Trim();
        jersey.Temporada = dto.Temporada.Trim();
        jersey.Tipo = dto.Tipo;
        jersey.Talla = dto.Talla;
        jersey.Estado = dto.Estado;
        jersey.Version = dto.Version;
        jersey.ValorEstimado = dto.ValorEstimado;
        jersey.FotoUrl = string.IsNullOrWhiteSpace(dto.FotoUrl) ? null : dto.FotoUrl.Trim();
    }

    private static JerseyResponseDto ToDto(Jersey j) => new()
    {
        IdJersey = j.IdJersey,
        Equipo = j.Equipo,
        Temporada = j.Temporada,
        Tipo = j.Tipo,
        Talla = j.Talla,
        Estado = j.Estado,
        Version = j.Version,
        ValorEstimado = j.ValorEstimado,
        FotoUrl = j.FotoUrl,
        Disponibilidad = j.Disponibilidad,
        FechaAlta = j.FechaAlta
    };
}
