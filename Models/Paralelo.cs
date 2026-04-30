using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Paralelos (secciones) de cada materia
/// </summary>
public partial class Paralelo
{
    public int Id { get; set; }

    public int MateriaId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Horario { get; set; } = null!;

    public string? Aula { get; set; }

    public int DocenteId { get; set; }

    public int Cupo { get; set; }

    public int? Inscritos { get; set; }

    public bool? Disponible { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Persona Docente { get; set; } = null!;

    public virtual Materia Materia { get; set; } = null!;
}
