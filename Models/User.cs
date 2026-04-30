using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

/// <summary>
/// Usuarios del sistema con autenticación básica
/// </summary>
public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EventoSuscripcione> EventoSuscripciones { get; set; } = new List<EventoSuscripcione>();

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual ICollection<Noticia> Noticia { get; set; } = new List<Noticia>();

    public virtual Persona? Persona { get; set; }
}
