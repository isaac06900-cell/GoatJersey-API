using GoatJersey_API.DTOs;
using GoatJersey_API.Helpers;
using GoatJersey_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoatJersey_API.Controllers;

[ApiController]
[Route("api/jerseys")]
[Authorize]
public class JerseysController : ControllerBase
{
    private readonly IJerseyService _service;

    public JerseysController(IJerseyService service)
    {
        _service = service;
    }

    // GET /api/jerseys -> mi colección
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var jerseys = await _service.ObtenerMiColeccionAsync(User.GetUserId());
        return Ok(jerseys);
    }

    // POST /api/jerseys
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] JerseyRequestDto dto)
    {
        var creado = await _service.CrearAsync(User.GetUserId(), dto);
        return Created($"/api/jerseys/{creado.IdJersey}", creado);
    }

    // PUT /api/jerseys/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] JerseyRequestDto dto)
    {
        await _service.ActualizarAsync(User.GetUserId(), id, dto);
        return NoContent();
    }

    // DELETE /api/jerseys/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(User.GetUserId(), id);
        return NoContent();
    }
}
