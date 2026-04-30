namespace DIMS_Backend.Features.Publicaciones;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class UpdatePublicacionHandler : IRequestHandler<UpdatePublicacionCommand, bool>
{
    private readonly UcbPortalContext _context;

    public UpdatePublicacionHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePublicacionCommand request, CancellationToken ct)
    {
        // Traemos la publicación incluyendo a la Persona y su UserId para validar
        var pub = await _context.Publicaciones
            .Include(p => p.Persona)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (pub == null) return false;

        // Lógica de Seguridad
        bool esAdmin = request.RequestUserRole == "admin";
        // Verificamos si la publicación pertenece a la persona vinculada al usuario logueado
        bool esDueno = pub.Persona != null && pub.Persona.UserId == request.RequestUserId;

        if (!esAdmin && !esDueno)
        {
            throw new UnauthorizedAccessException("No tienes permiso para editar esta publicación.");
        }

        // Actualización de campos
        pub.Titulo = request.Titulo;
        pub.Autor = request.Autor;
        pub.Resumen = request.Resumen;
        pub.ArchivoUrl = request.ArchivoUrl;
        pub.CarreraId = request.CarreraId;
        pub.Tipo = request.Tipo;

        // El trigger de tu DB se encargará del updated_at, 
        // pero EF Core lo enviará en el SaveChanges
        return await _context.SaveChangesAsync(ct) > 0;
    }
}