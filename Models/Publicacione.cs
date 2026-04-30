using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Publicaciones académicas (tesis, proyectos, artículos)
/// </summary>
public partial class Publicacione
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Autor { get; set; }

    public string? Resumen { get; set; }

    public string? ArchivoUrl { get; set; }

    public DateOnly? FechaPublicacion { get; set; }

    public int? CarreraId { get; set; }

    public int? PersonaId { get; set; }

    public string? Tipo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrera? Carrera { get; set; }

    public virtual Persona? Persona { get; set; }
}
