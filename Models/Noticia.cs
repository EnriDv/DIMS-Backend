using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Noticias y artículos del portal
/// </summary>
public partial class Noticia
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string? ImagenUrl { get; set; }

    public int? CarreraId { get; set; }

    public bool? Publicada { get; set; }

    public bool? Destacada { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrera? Carrera { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
