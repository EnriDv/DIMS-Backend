namespace DIMS_Backend.Features.Materias;

using MediatR;

public class CreateMateriaCommand : IRequest<int>
{
    public string Sigla { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int CarreraId { get; set; }
    public int? Semestre { get; set; }
    public string Tipo { get; set; } = "obligatoria";
    public string? Area { get; set; }
}

public record DeleteMateriaCommand(int Id) : IRequest<bool>;