using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Eventos académicos y extracurriculares
/// </summary>
public partial class Evento
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaEvento { get; set; }

    public string Lugar { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public int? CarreraId { get; set; }

    public int? Capacidad { get; set; }

    public int? Inscritos { get; set; }

    public bool? Publicado { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrera? Carrera { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<EventoSuscripcione> EventoSuscripciones { get; set; } = new List<EventoSuscripcione>();
}
