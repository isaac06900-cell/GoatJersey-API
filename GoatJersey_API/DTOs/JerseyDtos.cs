using System.ComponentModel.DataAnnotations;

namespace GoatJersey_API.DTOs;

/// <summary>Datos que se envían para crear o editar un jersey.</summary>
public class JerseyRequestDto
{
    [Required(ErrorMessage = "El equipo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El equipo no puede pasar de 100 caracteres.")]
    public string Equipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La temporada es obligatoria.")]
    [RegularExpression(@"^\d{4}/\d{2}$", ErrorMessage = "La temporada debe tener el formato 2009/10.")]
    public string Temporada { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [RegularExpression("^(Local|Visitante|Tercero|Portero|Retro|Especial)$",
        ErrorMessage = "Tipo inválido. Usa: Local, Visitante, Tercero, Portero, Retro o Especial.")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La talla es obligatoria.")]
    [RegularExpression("^(XS|S|M|L|XL|XXL)$", ErrorMessage = "Talla inválida. Usa: XS, S, M, L, XL o XXL.")]
    public string Talla { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [RegularExpression("^(Nuevo|Excelente|Bueno|Regular)$",
        ErrorMessage = "Estado inválido. Usa: Nuevo, Excelente, Bueno o Regular.")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "La versión es obligatoria.")]
    [RegularExpression("^(Original|Replica)$", ErrorMessage = "Versión inválida. Usa: Original o Replica.")]
    public string Version { get; set; } = string.Empty;

    [Range(0, 99999999, ErrorMessage = "El valor estimado no puede ser negativo.")]
    public decimal? ValorEstimado { get; set; }

    [Url(ErrorMessage = "La foto debe ser una URL válida (http o https).")]
    [StringLength(300, ErrorMessage = "La URL de la foto no puede pasar de 300 caracteres.")]
    public string? FotoUrl { get; set; }
}

/// <summary>Datos que devuelve la API de un jersey.</summary>
public class JerseyResponseDto
{
    public int IdJersey { get; set; }
    public string Equipo { get; set; } = string.Empty;
    public string Temporada { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Talla { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public decimal? ValorEstimado { get; set; }
    public string? FotoUrl { get; set; }
    public string Disponibilidad { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; }
}
