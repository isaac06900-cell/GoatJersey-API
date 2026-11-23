using System.ComponentModel.DataAnnotations;

namespace GoatJersey_API.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Coleccionista";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<Jersey> Jerseys { get; set; } = new List<Jersey>();
}
