using System;
using System.Collections.Generic;

namespace GestionarEventasos.Models;

public partial class Participante
{
    public int IdParticipante { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
