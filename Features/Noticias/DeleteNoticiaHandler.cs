// <copyright file="DeleteNoticiaHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DIMS_Backend.Features.Noticias;

using DIMS_Backend.Models;
using MediatR;

/// <summary>
/// Handler para borrado lógico de noticias.
/// </summary>
public class DeleteNoticiaHandler(UcbPortalContext context) : IRequestHandler<DeleteNoticiaCommand, bool>
{
    /// <summary>
    /// Ejecuta el borrado lógico de una noticia.
    /// </summary>
    /// <param name="request">Comando de borrado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si se aplicó el borrado lógico; en otro caso, false.</returns>
    public async Task<bool> Handle(DeleteNoticiaCommand request, CancellationToken cancellationToken)
    {
        var noticia = await context.Noticias.FindAsync(request.Id);
        if (noticia == null)
        {
            return false;
        }

        // Solo el creador o un admin pueden eliminar.
        if (request.RequestUserRole != "admin" && noticia.CreatedBy != request.RequestUserId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para eliminar esta noticia.");
        }

        noticia.Publicada = false;
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

