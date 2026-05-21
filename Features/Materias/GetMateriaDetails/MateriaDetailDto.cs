namespace DIMS_Backend.Features.Materias.GetMateriaDetails;

public class MateriaDetailDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int? Semestre { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;

    // Lista general de docentes que dan esta materia
    public List<DocenteResumenDto> Docentes { get; set; } = new();

    // Lista de paralelos abiertos con sus horarios y docente asignado
    public List<ParaleloDto> Paralelos { get; set; } = new();
}

public class DocenteResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}

public class ParaleloDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string? Aula { get; set; }
    public int DocenteId { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public int Cupo { get; set; }
    public int Inscritos { get; set; }
    public bool Disponible { get; set; }
}