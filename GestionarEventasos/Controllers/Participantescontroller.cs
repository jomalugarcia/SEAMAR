using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionarEventasos.Models;

namespace GestionarEventasos.Controllers
{
    public class ParticipantesController : Controller
    {
        private readonly SeamarContext _context;

        public ParticipantesController(SeamarContext context)
        {
            _context = context;
        }

        // GET: Participantes
        public async Task<IActionResult> Index(string? buscar)
        {
            var query = _context.Participantes.AsQueryable()
                .Include(p => p.Inscripciones)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(p =>
                    p.NombreCompleto.Contains(buscar) ||
                    p.Email.Contains(buscar));

            ViewBag.Buscar = buscar;
            return View(await query.OrderBy(p => p.NombreCompleto).ToListAsync());
        }

        // GET:  info
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var participante = await _context.Participantes
                .Include(p => p.Inscripciones)
                    .ThenInclude(i => i.Evento)
                .FirstOrDefaultAsync(p => p.IdParticipante == id);

            if (participante == null) return NotFound();
            return View(participante);
        }

        // GET:  crear
        public IActionResult Create() => View();

        // POST:  crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("NombreCompleto,Email,Telefono,FechaRegistro")] Participante participante)
        {
            if (await _context.Participantes.AnyAsync(p => p.Email == participante.Email))
                ModelState.AddModelError("Email", "Ya existe un participante con este correo electrónico.");

            if (ModelState.IsValid)
            {
                _context.Add(participante);
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"\"{participante.NombreCompleto}\" registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(participante);
        }

        // GET:  editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var p = await _context.Participantes.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST:  editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdParticipante,NombreCompleto,Email,Telefono,FechaRegistro")] Participante participante)
        {
            if (id != participante.IdParticipante) return NotFound();

            if (await _context.Participantes.AnyAsync(p =>
                    p.Email == participante.Email && p.IdParticipante != participante.IdParticipante))
                ModelState.AddModelError("Email", "Ya existe otro participante con este correo electrónico.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(participante);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Participante actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Participantes.Any(p => p.IdParticipante == participante.IdParticipante))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(participante);
        }

        // GET:  borrar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var participante = await _context.Participantes
                .Include(p => p.Inscripciones)
                .FirstOrDefaultAsync(p => p.IdParticipante == id);
            if (participante == null) return NotFound();
            return View(participante);
        }

        // POST:  borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _context.Participantes.FindAsync(id);
            if (p != null)
            {
                _context.Participantes.Remove(p);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Participante eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}