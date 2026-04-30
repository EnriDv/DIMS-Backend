namespace DIMS_Backend.Features.Carreras.GetMallaCurricular;

public class MateriaMallaDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int? Semestre { get; set; }
    public string Tipo { get; set; } = string.Empty; // obligatoria, electiva
    public string? Area { get; set; }
}