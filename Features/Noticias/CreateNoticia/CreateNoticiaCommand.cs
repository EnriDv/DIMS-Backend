namespace DIMS_Backend.Features.Noticias.CreateNoticia;

using MediatR;

public class CreateNoticiaCommand : IRequest<int>
{
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public int? CarreraId { get; set; }
    public bool Destacada { get; set; }
    public Guid? CreatedBy { get; set; }
}