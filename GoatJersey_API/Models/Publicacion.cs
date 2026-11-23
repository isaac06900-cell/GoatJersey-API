using System.ComponentModel.DataAnnotations;

namespace GoatJersey_API.Models;

public class Publicacion
{
    [Key]
    public int IdPublicacion { get; set; }
    public int IdJersey { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal? Precio { get; set; }
    public string? Descripcion { get; set; }
    public string Estatus { get; set; } = "Activa";
    public DateTime FechaPublicacion { get; set; } = DateTime.Now;

    public Jersey? Jersey { get; set; }
}
