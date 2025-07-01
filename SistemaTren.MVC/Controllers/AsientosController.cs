using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTren.MVC.Data;
using SistemaVentaBoletosTrenes.Modelo;

namespace SistemaTren.MVC.Controllers
{
    [Authorize]
    public class AsientosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Asientos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Asientos
                .OrderBy(a => a.NumeroAsiento)
                .ToListAsync());
        }

        // GET: Asientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asiento = await _context.Asientos
                .FirstOrDefaultAsync(m => m.AsientoID == id);
            if (asiento == null)
            {
                return NotFound();
            }

            return View(asiento);
        }

        // GET: Asientos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Asientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AsientoID,NumeroAsiento,TipoAsiento,Disponible")] Asiento asiento)
        {
            if (_context.Asientos.Any(a => a.NumeroAsiento == asiento.NumeroAsiento))
            {
                ModelState.AddModelError("NumeroAsiento", "Este número de asiento ya existe");
            }

            if (ModelState.IsValid)
            {
                _context.Add(asiento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asiento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(asiento);
        }

        // GET: Asientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asiento = await _context.Asientos.FindAsync(id);
            if (asiento == null)
            {
                return NotFound();
            }
            return View(asiento);
        }

        // POST: Asientos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AsientoID,NumeroAsiento,TipoAsiento,Disponible")] Asiento asiento)
        {
            if (id != asiento.AsientoID)
            {
                return NotFound();
            }

            if (_context.Asientos.Any(a => a.NumeroAsiento == asiento.NumeroAsiento && a.AsientoID != id))
            {
                ModelState.AddModelError("NumeroAsiento", "Este número de asiento ya existe");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asiento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asiento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsientoExists(asiento.AsientoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(asiento);
        }

        // GET: Asientos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asiento = await _context.Asientos
                .Include(a => a.Boletos)
                .FirstOrDefaultAsync(m => m.AsientoID == id);

            if (asiento == null)
            {
                return NotFound();
            }

            ViewBag.TieneBoletos = asiento.Boletos?.Any() == true;
            return View(asiento);
        }

        // POST: Asientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var asiento = await _context.Asientos
                    .Include(a => a.Boletos)
                    .FirstOrDefaultAsync(a => a.AsientoID == id);

                if (asiento == null)
                {
                    TempData["ErrorMessage"] = "El asiento no fue encontrado.";
                    return View("DeleteError");
                }

                if (asiento.Boletos?.Any() == true)
                {
                    TempData["ErrorMessage"] = $"No se puede eliminar el asiento {asiento.NumeroAsiento} porque está asignado a {asiento.Boletos.Count} boleto(s).";
                    return View("DeleteError");
                }

                _context.Asientos.Remove(asiento);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Asiento {asiento.NumeroAsiento} eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado al eliminar el asiento: {ex.Message}";
                return View("DeleteError");
            }
        }

        // GET: Asientos/DeleteError
        public IActionResult DeleteError()
        {
            return View();
        }

        private bool AsientoExists(int id)
        {
            return _context.Asientos.Any(e => e.AsientoID == id);
        }
    }
}