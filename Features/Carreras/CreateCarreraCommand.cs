namespace DIMS_Backend.Features.Carreras;

using MediatR;

public class CreateCarreraCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Duracion { get; set; }
    public string? Modalidad { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }
}

public class UpdateCarreraCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
}

public record DeleteCarreraCommand(int Id) : IRequest<bool>;