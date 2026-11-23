using System.ComponentModel.DataAnnotations;

namespace GoatJersey_API.Models;

public class Jersey
{
    [Key]
    public int IdJersey { get; set; }
    public int IdUsuario { get; set; }
    public string Equipo { get; set; } = string.Empty;
    public string Temporada { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Talla { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public decimal? ValorEstimado { get; set; }
    public string? FotoUrl { get; set; }
    public string Disponibilidad { get; set; } = "EnColeccion";
    public DateTime FechaAlta { get; set; } = DateTime.Now;

    public Usuario? Usuario { get; set; }
    public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
}
