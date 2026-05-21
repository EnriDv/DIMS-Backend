using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Docentes, directores, coordinadores e investigadores
/// </summary>
public partial class Persona
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Rol { get; set; } = null!;

    public int CarreraId { get; set; }

    public string? FotoUrl { get; set; }

    public string? Bio { get; set; }

    public string? Especialidad { get; set; }

    public string? GradoAcademico { get; set; }

    public int? PublicacionesCount { get; set; }

    public string? LinkedinUrl { get; set; }

    public string? GoogleScholarUrl { get; set; }

    public bool? Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UserId { get; set; }

    public virtual Carrera Carrera { get; set; } = null!;

    public virtual ICollection<Paralelo> Paralelos { get; set; } = new List<Paralelo>();

    public virtual ICollection<Publicacione> Publicaciones { get; set; } = new List<Publicacione>();

    public virtual User? User { get; set; }

    public virtual ICollection<Materia> Materia { get; set; } = new List<Materia>();
}
