using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionarEventasos.Models;

namespace GestionarEventasos.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly SeamarContext _context;

        public InscripcionesController(SeamarContext context)
        {
            _context = context;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index(int? eventoId)
        {
            var query = _context.Inscripciones
                .Include(i => i.Evento)
                .Include(i => i.Participante)
                .AsQueryable();

            if (eventoId.HasValue)
                query = query.Where(i => i.EventoId == eventoId.Value);

            ViewBag.Eventos = new SelectList(
                await _context.Eventos.OrderBy(e => e.Nombre).ToListAsync(),
                "IdEvento", "Nombre", eventoId);
            ViewBag.EventoFiltro = eventoId;

            return View(await query.OrderByDescending(i => i.FechaInscripcion).ToListAsync());
        }

        // GET:  crear
        public async Task<IActionResult> Create(int? eventoId)
        {
            await CargarSelectLists(eventoId);
            var inscripcion = new Inscripcione 
            {
                EventoId = eventoId ?? 0,
                FechaInscripcion = DateTime.Now,
                SeVino = false
            };
            return View(inscripcion);
        }

        // POST:  crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EventoId,ParticipanteId,FechaInscripcion,SeVino")] Inscripcione inscripcion)
        {
            if (ModelState.IsValid)
            {
                
                var evento = await _context.Eventos
                    .Include(e => e.Inscripciones)
                    .FirstOrDefaultAsync(e => e.IdEvento == inscripcion.EventoId);

                if (evento == null)
                {
                    ModelState.AddModelError("EventoId", "El evento no existe.");
                    await CargarSelectLists();
                    return View(inscripcion);
                }

              
                if (evento.Estado == 1)
                {
                    ModelState.AddModelError("EventoId", "No se puede inscribir en un evento cancelado.");
                    await CargarSelectLists(inscripcion.EventoId);
                    return View(inscripcion);
                }

                
                if (evento.Inscripciones.Count >= evento.Cupos)
                {
                    ModelState.AddModelError(string.Empty,
                        $"❌ El evento \"{evento.Nombre}\" ya alcanzó su límite de {evento.Cupos} cupo(s). " +
                        $"No es posible agregar más inscripciones.");
                    await CargarSelectLists(inscripcion.EventoId);
                    return View(inscripcion);
                }

               
                bool yaInscrito = await _context.Inscripciones.AnyAsync(i =>
                    i.EventoId == inscripcion.EventoId &&
                    i.ParticipanteId == inscripcion.ParticipanteId);

                if (yaInscrito)
                {
                    var par = await _context.Participantes.FindAsync(inscripcion.ParticipanteId);
                    ModelState.AddModelError(string.Empty,
                        $"❌ \"{par?.NombreCompleto}\" ya está inscrito en este evento.");
                    await CargarSelectLists(inscripcion.EventoId);
                    return View(inscripcion);
                }

                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Inscripción realizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarSelectLists(inscripcion.EventoId);
            return View(inscripcion);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null) return NotFound();
            await CargarSelectLists(inscripcion.EventoId, inscripcion.ParticipanteId);
            return View(inscripcion);
        }

        // POST:  editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdInscripcion,EventoId,ParticipanteId,FechaInscripcion,SeVino")] Inscripcione inscripcion)
        {
            if (id != inscripcion.IdInscripcion) return NotFound();

            if (ModelState.IsValid)
            {
                bool duplicado = await _context.Inscripciones.AnyAsync(i =>
                    i.EventoId == inscripcion.EventoId &&
                    i.ParticipanteId == inscripcion.ParticipanteId &&
                    i.IdInscripcion != inscripcion.IdInscripcion);

                if (duplicado)
                {
                    ModelState.AddModelError(string.Empty,
                        "❌ Este participante ya está inscrito en ese evento.");
                    await CargarSelectLists(inscripcion.EventoId, inscripcion.ParticipanteId);
                    return View(inscripcion);
                }

                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Inscripción actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inscripciones.Any(i => i.IdInscripcion == inscripcion.IdInscripcion))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await CargarSelectLists(inscripcion.EventoId, inscripcion.ParticipanteId);
            return View(inscripcion);
        }

        // GET:  borrar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Evento)
                .Include(i => i.Participante)
                .FirstOrDefaultAsync(i => i.IdInscripcion == id);
            if (inscripcion == null) return NotFound();
            return View(inscripcion);
        }

        // POST:  borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Inscripción eliminada.";
            }
            return RedirectToAction(nameof(Index));
        }

      
        private async Task CargarSelectLists(int? eventoSel = null, int? participanteSel = null)
        {
            ViewBag.EventoId = new SelectList(
                await _context.Eventos
                    .Where(e => e.Estado == 0)   
                    .OrderBy(e => e.Nombre)
                    .ToListAsync(),
                "IdEvento", "Nombre", eventoSel);

            ViewBag.ParticipanteId = new SelectList(
                await _context.Participantes
                    .OrderBy(p => p.NombreCompleto)
                    .ToListAsync(),
                "IdParticipante", "NombreCompleto", participanteSel);
        }
    }
}