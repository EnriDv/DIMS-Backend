using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Carreras universitarias ofrecidas
/// </summary>
public partial class Carrera
{
    public int Id { get; set; }

    public string Slug { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Duracion { get; set; }

    public string? Modalidad { get; set; }

    public string? ImagenUrl { get; set; }

    public string? Color { get; set; }

    public string? Icono { get; set; }

    public bool? Activa { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CampoLaboral> CampoLaborals { get; set; } = new List<CampoLaboral>();

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual ICollection<Materia> Materia { get; set; } = new List<Materia>();

    public virtual ICollection<Noticia> Noticia { get; set; } = new List<Noticia>();

    public virtual ICollection<PerfilEgresado> PerfilEgresados { get; set; } = new List<PerfilEgresado>();

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    public virtual ICollection<Publicacione> Publicaciones { get; set; } = new List<Publicacione>();
}
