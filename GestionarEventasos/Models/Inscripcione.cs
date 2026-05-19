using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionarEventasos.Models;

public partial class Inscripcione
{
    public int IdInscripcion { get; set; }

    public int EventoId { get; set; }

    public int ParticipanteId { get; set; }

    public DateTime FechaInscripcion { get; set; }

    public bool SeVino { get; set; }

    [ValidateNever]
    public virtual Evento Evento { get; set; }

    [ValidateNever]
    public virtual Participante Participante { get; set; } 
}
