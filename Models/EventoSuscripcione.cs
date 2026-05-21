using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

public partial class EventoSuscripcione
{
    public int EventoId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? FechaSuscripcion { get; set; }

    public virtual Evento Evento { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
