using GoatJersey_API.DTOs;
using GoatJersey_API.Helpers;
using GoatJersey_API.Models;
using GoatJersey_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoatJersey_API.Services;

public interface IPublicacionService
{
    Task<List<PublicacionResponseDto>> ExplorarAsync(int idUsuario, PublicacionFiltrosDto filtros);
    Task<PublicacionResponseDto> ObtenerDetalleAsync(int idPublicacion);
    Task<PublicacionResponseDto> PublicarAsync(int idUsuario, PublicacionRequestDto dto);
    Task RetirarAsync(int idUsuario, bool esAdmin, int idPublicacion);
}

public class PublicacionService : IPublicacionService
{
    private readonly IPublicacionRepository _repository;
    private readonly IJerseyRepository _jerseyRepository;

    public PublicacionService(IPublicacionRepository repository, IJerseyRepository jerseyRepository)
    {
        _repository = repository;
        _jerseyRepository = jerseyRepository;
    }

    public async Task<List<PublicacionResponseDto>> ExplorarAsync(int idUsuario, PublicacionFiltrosDto filtros)
    {
        var lista = await _repository.ObtenerActivasDeOtrosAsync(idUsuario, filtros);
        return lista.Select(ToDto).ToList();
    }

    public async Task<PublicacionResponseDto> ObtenerDetalleAsync(int idPublicacion)
    {
        var publicacion = await _repository.ObtenerDetalleAsync(idPublicacion)
                          ?? throw new NotFoundException("La publicación no existe.");
        return ToDto(publicacion);
    }

    public async Task<PublicacionResponseDto> PublicarAsync(int idUsuario, PublicacionRequestDto dto)
    {
        var jersey = await _jerseyRepository.ObtenerPorIdAsync(dto.IdJersey)
                     ?? throw new NotFoundException("El jersey no existe.");

        if (jersey.IdUsuario != idUsuario)
            throw new ForbiddenException("No puedes publicar un jersey que no es tuyo.");

        if (await _repository.TieneActivaAsync(dto.IdJersey))
            throw new BusinessRuleException("Este jersey ya tiene una publicación activa.");

        var conVenta = dto.Tipo is "Venta" or "Ambos";
        if (conVenta && (dto.Precio is null || dto.Precio <= 0))
            throw new BusinessRuleException("El precio es obligatorio y debe ser mayor que 0 cuando hay venta.");

        var publicacion = new Publicacion
        {
            IdJersey = jersey.IdJersey,
            Tipo = dto.Tipo,
            Precio = conVenta ? dto.Precio : null,   // en Intercambio no se guarda precio
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
            Estatus = "Activa",
            FechaPublicacion = DateTime.Now,
            Jersey = jersey
        };

        // El jersey ya viene con seguimiento del mismo DbContext, así que este SaveChanges
        // guarda la publicación y el cambio de Disponibilidad en una sola transacción.
        jersey.Disponibilidad = "Publicado";
        await _repository.AgregarAsync(publicacion);

        try
        {
            await _repository.GuardarCambiosAsync();
        }
        catch (DbUpdateException)
        {
            // Dos peticiones al mismo tiempo: el índice único filtrado de la BD evita la segunda.
            throw new BusinessRuleException("Este jersey ya tiene una publicación activa.");
        }

        return await ObtenerDetalleAsync(publicacion.IdPublicacion);
    }

    public async Task RetirarAsync(int idUsuario, bool esAdmin, int idPublicacion)
    {
        var publicacion = await _repository.ObtenerPorIdAsync(idPublicacion)
                          ?? throw new NotFoundException("La publicación no existe.");

        // Puede retirarla su dueño o un Admin (moderación).
        if (publicacion.Jersey!.IdUsuario != idUsuario && !esAdmin)
            throw new ForbiddenException("No tienes permiso para retirar esta publicación.");

        if (publicacion.Estatus == "Retirada")
            throw new BusinessRuleException("La publicación ya estaba retirada.");

        publicacion.Estatus = "Retirada";
        publicacion.Jersey.Disponibilidad = "EnColeccion";   // el jersey vuelve a la colección
        await _repository.GuardarCambiosAsync();
    }

    private static PublicacionResponseDto ToDto(Publicacion p) => new()
    {
        IdPublicacion = p.IdPublicacion,
        Tipo = p.Tipo,
        Precio = p.Precio,
        Descripcion = p.Descripcion,
        Estatus = p.Estatus,
        FechaPublicacion = p.FechaPublicacion,
        Jersey = new JerseyResponseDto
        {
            IdJersey = p.Jersey!.IdJersey,
            Equipo = p.Jersey.Equipo,
            Temporada = p.Jersey.Temporada,
            Tipo = p.Jersey.Tipo,
            Talla = p.Jersey.Talla,
            Estado = p.Jersey.Estado,
            Version = p.Jersey.Version,
            ValorEstimado = p.Jersey.ValorEstimado,
            FotoUrl = p.Jersey.FotoUrl,
            Disponibilidad = p.Jersey.Disponibilidad,
            FechaAlta = p.Jersey.FechaAlta
        },
        Vendedor = new VendedorDto
        {
            IdUsuario = p.Jersey.IdUsuario,
            Nombre = p.Jersey.Usuario?.Nombre ?? string.Empty,
            Correo = p.Jersey.Usuario?.Correo ?? string.Empty
        }
    };
}