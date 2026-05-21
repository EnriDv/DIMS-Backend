namespace DIMS_Backend.Features.Personas.GetPersonas;

public class PersonaListDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? Especialidad { get; set; }
    public string CarreraNombre { get; set; } = string.Empty;
}