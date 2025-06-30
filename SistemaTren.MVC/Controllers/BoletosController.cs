using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTren.MVC.Data;
using SistemaTren.MVC.Service;
using SistemaVentaBoletosTrenes.Modelo;

namespace SistemaTren.MVC.Controllers
{
    [Authorize]
    public class BoletosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FactoryBoleto _factoryBoleto;
        private readonly NotificacionService _notificacionService;
        private readonly PrecioService _precioService;

        public BoletosController(ApplicationDbContext context,
                               FactoryBoleto factoryBoleto,
                               NotificacionService notificacionService,
                               PrecioService precioService)
        {
            _context = context;
            _factoryBoleto = factoryBoleto;
            _notificacionService = notificacionService;
            _precioService = precioService;
        }

        // GET: Boletos/Create
        [HttpGet]
        public IActionResult Create(int? rutaId)
        {
            // Cargar datos necesarios para los dropdowns
            ViewBag.Rutas = _context.Rutas.ToList();
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Asientos = _context.Asientos.ToList();

            // Si se proporciona un rutaId, establecerlo como seleccionado
            if (rutaId.HasValue)
            {
                ViewBag.RutaSeleccionada = rutaId.Value;
            }

            return View();
        }

        // POST: Boletos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoletoID,Precio,FechaCompra,RutaID,CategoriaID,AsientoID")] Boleto boleto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener datos relacionados
                    var ruta = await _context.Rutas.FindAsync(boleto.RutaID);
                    var categoria = await _context.Categorias.FindAsync(boleto.CategoriaID);
                    var asiento = await _context.Asientos.FindAsync(boleto.AsientoID);

                    if (ruta == null || categoria == null || asiento == null)
                    {
                        ModelState.AddModelError("", "Datos inválidos. Por favor verifique la información.");
                        return View(boleto);
                    }

                    // Usar FactoryBoleto para crear el boleto
                    var boletoCreado = _factoryBoleto.CrearBoleto(ruta, categoria, asiento);

                    // Calcular el precio final usando PrecioService
                    boletoCreado.Precio = _precioService.CalcularPrecio(boletoCreado);
                    boletoCreado.FechaCompra = DateTime.Now;

                    // Guardar en la base de datos
                    _context.Add(boletoCreado);
                    await _context.SaveChangesAsync();

                    // Notificar al cliente
                    _notificacionService.EnviarNotificacion($"¡Boleto comprado con éxito! Número de boleto: {boletoCreado.BoletoID}", this);

                    return RedirectToAction(nameof(Confirmacion), new { id = boletoCreado.BoletoID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al procesar su solicitud. Por favor intente nuevamente.");
                    // Log the exception
                    Console.WriteLine(ex.Message);
                }
            }

            // Si hay errores, recargar los dropdowns
            ViewBag.Rutas = _context.Rutas.ToList();
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Asientos = _context.Asientos.ToList();
            return View(boleto);
        }

        // GET: Boletos/Confirmacion/5
        public async Task<IActionResult> Confirmacion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boleto = await _context.Boletos
                .Include(b => b.Ruta)
                .Include(b => b.Categoria)
                .Include(b => b.Asiento)
                .FirstOrDefaultAsync(m => m.BoletoID == id);

            if (boleto == null)
            {
                return NotFound();
            }

            return View(boleto);
        }

        // GET: Boletos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Boletos.Include(b => b.Asiento).Include(b => b.Categoria).Include(b => b.Ruta);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Boletos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boleto = await _context.Boletos
                .Include(b => b.Asiento)
                .Include(b => b.Categoria)
                .Include(b => b.Ruta)
                .FirstOrDefaultAsync(m => m.BoletoID == id);
            if (boleto == null)
            {
                return NotFound();
            }

            return View(boleto);
        }


        // GET: Boletos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boleto = await _context.Boletos.FindAsync(id);
            if (boleto == null)
            {
                return NotFound();
            }
            ViewData["AsientoID"] = new SelectList(_context.Asientos, "AsientoID", "TipoAsiento", boleto.AsientoID);
            ViewData["CategoriaID"] = new SelectList(_context.Set<Categoria>(), "CategoriaID", "Nombre", boleto.CategoriaID);
            ViewData["RutaID"] = new SelectList(_context.Set<Ruta>(), "RutaID", "Destino", boleto.RutaID);
            return View(boleto);
        }

        // POST: Boletos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoletoID,Precio,FechaCompra,RutaID,CategoriaID,AsientoID")] Boleto boleto)
        {
            if (id != boleto.BoletoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boleto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoletoExists(boleto.BoletoID))
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
            ViewData["AsientoID"] = new SelectList(_context.Asientos, "AsientoID", "TipoAsiento", boleto.AsientoID);
            ViewData["CategoriaID"] = new SelectList(_context.Set<Categoria>(), "CategoriaID", "Nombre", boleto.CategoriaID);
            ViewData["RutaID"] = new SelectList(_context.Set<Ruta>(), "RutaID", "Destino", boleto.RutaID);
            return View(boleto);
        }

        // GET: Boletos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boleto = await _context.Boletos
                .Include(b => b.Asiento)
                .Include(b => b.Categoria)
                .Include(b => b.Ruta)
                .FirstOrDefaultAsync(m => m.BoletoID == id);
            if (boleto == null)
            {
                return NotFound();
            }


            return View(boleto);
        }

        // POST: Boletos/Delete/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boleto = await _context.Boletos.FindAsync(id);
            if (boleto != null)
            {
                _context.Boletos.Remove(boleto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoletoExists(int id)
        {
            return _context.Boletos.Any(e => e.BoletoID == id);
        }
    }
}
