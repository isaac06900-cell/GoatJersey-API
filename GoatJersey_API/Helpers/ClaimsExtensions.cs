using System.Security.Claims;

namespace GoatJersey_API.Helpers;

public static class ClaimsExtensions
{
    /// <summary>Obtiene el id del usuario autenticado desde el token (claim sub).</summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value;

        if (!int.TryParse(value, out var id))
            throw new AppUnauthorizedException("El token no es válido.");

        return id;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
}
