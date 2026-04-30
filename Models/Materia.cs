using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Materias del plan de estudios
/// </summary>
public partial class Materia
{
    public int Id { get; set; }

    public string Sigla { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Creditos { get; set; }

    public int CarreraId { get; set; }

    public int? Semestre { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Area { get; set; }

    public bool? Activa { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrera Carrera { get; set; } = null!;

    public virtual ICollection<Paralelo> Paralelos { get; set; } = new List<Paralelo>();

    public virtual ICollection<Persona> Docentes { get; set; } = new List<Persona>();

    public virtual ICollection<Materia> MateriaNavigation { get; set; } = new List<Materia>();

    public virtual ICollection<Materia> Requisitos { get; set; } = new List<Materia>();
}
