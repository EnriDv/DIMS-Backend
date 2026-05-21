using System;
using System.Collections.Generic;

namespace DIMS_Backend.Models;

public partial class CampoLaboral
{
    public int Id { get; set; }

    public int CarreraId { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? Orden { get; set; }

    public virtual Carrera Carrera { get; set; } = null!;
}
