using GoatJersey_API.DTOs;
using GoatJersey_API.Helpers;
using GoatJersey_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoatJersey_API.Controllers;

[ApiController]
[Route("api/publicaciones")]
[Authorize]
public class PublicacionesController : ControllerBase
{
    private readonly IPublicacionService _service;

    public PublicacionesController(IPublicacionService service)
    {
        _service = service;
    }

    // GET /api/publicaciones?equipo=&temporada=&talla=&tipo=&version=
    [HttpGet]
    public async Task<IActionResult> Explorar([FromQuery] PublicacionFiltrosDto filtros)
    {
        var lista = await _service.ExplorarAsync(User.GetUserId(), filtros);
        return Ok(lista);
    }

    // GET /api/publicaciones/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id)
    {
        var publicacion = await _service.ObtenerDetalleAsync(id);
        return Ok(publicacion);
    }

    // POST /api/publicaciones
    [HttpPost]
    public async Task<IActionResult> Publicar([FromBody] PublicacionRequestDto dto)
    {
        var creada = await _service.PublicarAsync(User.GetUserId(), dto);
        return Created($"/api/publicaciones/{creada.IdPublicacion}", creada);
    }

    // PATCH /api/publicaciones/{id}/retirar
    [HttpPatch("{id:int}/retirar")]
    public async Task<IActionResult> Retirar(int id)
    {
        await _service.RetirarAsync(User.GetUserId(), User.IsAdmin(), id);
        return NoContent();
    }
}