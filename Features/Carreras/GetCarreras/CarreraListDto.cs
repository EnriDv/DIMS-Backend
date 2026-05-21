namespace DIMS_Backend.Features.Carreras.GetCarreras;

public class CarreraListDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Duracion { get; set; }
    public string? Modalidad { get; set; }
    public string? ImagenUrl { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }
}