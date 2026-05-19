using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionarEventasos.Models;

namespace GestionarEventasos.Controllers
{
    public class EventosController : Controller
    {
        private readonly SeamarContext _context;

        public EventosController(SeamarContext context)
        {
            _context = context;
        }

        // GET: Eventos
        public async Task<IActionResult> Index(string? buscar, string? estado)
        {
            var query = _context.Eventos.Include(e => e.Inscripciones).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(e => e.Nombre.Contains(buscar) || e.Lugar.Contains(buscar));

            if (!string.IsNullOrWhiteSpace(estado) && int.TryParse(estado, out int estadoInt))
                query = query.Where(e => e.Estado == estadoInt);

            ViewBag.Buscar = buscar;
            ViewBag.EstadoFiltro = estado;

            return View(await query.OrderByDescending(e => e.Fecha).ToListAsync());
        }

        // GET: detalle
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Inscripciones)
                    .ThenInclude(i => i.Participante)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null) return NotFound();
            return View(evento);
        }

        // GET: Craer
        public IActionResult Create() => View();

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Fecha,Lugar,Cupos,Estado")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"Evento \"{evento.Nombre}\" creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: Edita
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return NotFound();
            return View(evento);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEvento,Nombre,Fecha,Lugar,Cupos,Estado")] Evento evento)
        {
            if (id != evento.IdEvento) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Evento actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Eventos.Any(e => e.IdEvento == evento.IdEvento)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: borrar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var evento = await _context.Eventos
                .Include(e => e.Inscripciones)
                .FirstOrDefaultAsync(e => e.IdEvento == id);
            if (evento == null) return NotFound();
            return View(evento);
        }

        // POST: borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Evento eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}