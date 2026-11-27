using System.ComponentModel.DataAnnotations;

namespace GoatJersey_API.DTOs;

/// <summary>Datos que se envían para publicar un jersey.</summary>
public class PublicacionRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El id del jersey es obligatorio.")]
    public int IdJersey { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [RegularExpression("^(Venta|Intercambio|Ambos)$",
        ErrorMessage = "Tipo inválido. Usa: Venta, Intercambio o Ambos.")]
    public string Tipo { get; set; } = string.Empty;

    [Range(0.01, 99999999, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal? Precio { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede pasar de 500 caracteres.")]
    public string? Descripcion { get; set; }
}

/// <summary>Datos públicos del vendedor que se muestran en una publicación.</summary>
public class VendedorDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
}

/// <summary>Filtros opcionales del listado de publicaciones (query string).</summary>
public class PublicacionFiltrosDto
{
    public string? Equipo { get; set; }
    public string? Temporada { get; set; }
    public string? Talla { get; set; }
    public string? Tipo { get; set; }
    public string? Version { get; set; }
}

/// <summary>Publicación con los datos del jersey y del vendedor.</summary>
public class PublicacionResponseDto
{
    public int IdPublicacion { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal? Precio { get; set; }
    public string? Descripcion { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public JerseyResponseDto Jersey { get; set; } = new();
    public VendedorDto Vendedor { get; set; } = new();
}
