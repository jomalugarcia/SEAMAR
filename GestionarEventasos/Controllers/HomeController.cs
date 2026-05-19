using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionarEventasos.Models;

namespace GestionarEventasos.Controllers
{
    public class HomeController : Controller
    {
        private readonly SeamarContext _context;

        public HomeController(SeamarContext context)
        {
            _context = context;
        }

       //Esto permite abrir lo que es el Menu principal, aunque no jala sinceramente JAJAJAJA
        public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Eventos
                .Include(e => e.Inscripciones)
                .Where(e => e.Estado == 0 && e.Fecha >= DateTime.Now);

            if (fechaDesde.HasValue)
                query = query.Where(e => e.Fecha >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(e => e.Fecha <= fechaHasta.Value.AddDays(1));

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(await query.OrderBy(e => e.Fecha).ToListAsync());
        }

       
        public async Task<IActionResult> VerEvento(int? id)
        {
            if (id == null) return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Inscripciones)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null) return NotFound();

            return View(evento);
        }
        //Por que me enfoque que jalara aca,en lo de reporte, quizas no sea correcto pero asi termino
        public async Task<IActionResult> Reporte(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Eventos
                .Include(e => e.Inscripciones)
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(e => e.Fecha >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(e => e.Fecha <= fechaHasta.Value.AddDays(1));

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            var reporte = await query
                .OrderBy(e => e.Fecha)
                .Select(e => new ReporteEventoViewModel
                {
                    EventoId = e.IdEvento,
                    Nombre = e.Nombre,
                    Fecha = e.Fecha,
                    Lugar = e.Lugar,
                    Cupos = e.Cupos,
                    Inscritos = e.Inscripciones.Count(),
                    Asistentes = e.Inscripciones.Count(i => i.SeVino == true),
                    Estado = e.Estado
                })
                .ToListAsync();

            return View(reporte);
        }

        public IActionResult Error() => View();
    }
}