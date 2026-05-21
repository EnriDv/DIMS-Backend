namespace DIMS_Backend.Features.Personas.GetPersonaDetails;

public class PersonaDetailDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? FotoUrl { get; set; }
    public string? Especialidad { get; set; }
    public string? GradoAcademico { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? GoogleScholarUrl { get; set; }
    public string CarreraBase { get; set; } = string.Empty;
    public List<string> Areas { get; set; } = new();
    public List<MateriaDocenteDto> Materias { get; set; } = new();
}

public class MateriaDocenteDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Area { get; set; }
}