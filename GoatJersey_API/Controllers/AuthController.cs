using GoatJersey_API.DTOs;
using GoatJersey_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoatJersey_API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        var mensaje = await _authService.RegistrarAsync(dto);
        return StatusCode(StatusCodes.Status201Created, new { mensaje });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        return Ok(new { token });
    }
}
