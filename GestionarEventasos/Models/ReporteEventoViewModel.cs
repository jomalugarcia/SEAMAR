namespace GestionarEventasos.Models
{
    public class ReporteEventoViewModel
    {
        public int      EventoId    { get; set; }
        public string   Nombre      { get; set; } = string.Empty;
        public DateTime Fecha       { get; set; }
        public string   Lugar       { get; set; } = string.Empty;
        public int      Cupos       { get; set; }
        public int      Inscritos   { get; set; }
        public int      Asistentes  { get; set; }
        public int      Estado      { get; set; }

        public int    Disponibles         => Cupos - Inscritos;
        public double PorcentajeOcupacion =>
            Cupos > 0 ? Math.Round((double)Inscritos / Cupos * 100, 1) : 0;

        public string EstadoTexto => Estado switch
        {
            0 => "Activo", 1 => "Cancelado", 2 => "Finalizado", _ => "—"
        };
    }
}
