namespace DIMS_Backend.Features.Noticias;

using MediatR;

public class UpdateNoticiaCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public int? CarreraId { get; set; }
    public bool Destacada { get; set; }
    public bool Publicada { get; set; } = true;
    // Este campo lo llenaremos desde el Controller usando el JWT
    public Guid RequestUserId { get; set; }
    public string RequestUserRole { get; set; } = string.Empty;
}