using System;
using System.Collections.Generic;

namespace GestionarEventasos.Models;

public partial class Evento
{
    public int IdEvento { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string Lugar { get; set; } = null!;

    public int Cupos { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
