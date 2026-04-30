namespace DIMS_Backend.Features.Carreras.GetCarreraDetails;

public class CarreraDetailDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Duracion { get; set; }
    public string? Modalidad { get; set; }
    public string? ImagenUrl { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }

    // Arrays para pintar fácilmente en tu frontend
    public List<string> PerfilEgresado { get; set; } = new();
    public List<string> CampoLaboral { get; set; } = new();
}