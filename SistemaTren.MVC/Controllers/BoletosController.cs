using System;
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
            // Cargar listas para dropdowns
            ViewBag.Rutas = _context.Rutas.ToList();
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.AsientosDisponibles = _context.Asientos
                .Where(a => a.Disponible)
                .OrderBy(a => a.NumeroAsiento)
                .ToList();

            // Si viene con ruta preseleccionada
            if (rutaId.HasValue)
            {
                ViewBag.RutaSeleccionada = rutaId.Value;
            }

            return View();
        }

        // POST: Boletos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoletoID,RutaID,CategoriaID,AsientoID")] Boleto boleto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar y obtener datos relacionados
                    var asiento = await _context.Asientos.FindAsync(boleto.AsientoID);
                    if (asiento == null || !asiento.Disponible)
                    {
                        ModelState.AddModelError("AsientoID", "El asiento seleccionado no está disponible");
                        return await ReloadCreateView(boleto);
                    }

                    var ruta = await _context.Rutas.FindAsync(boleto.RutaID);
                    var categoria = await _context.Categorias.FindAsync(boleto.CategoriaID);

                    if (ruta == null || categoria == null)
                    {
                        ModelState.AddModelError("", "Datos inválidos. Verifique la información.");
                        return await ReloadCreateView(boleto);
                    }

                    // Marcar asiento como ocupado
                    asiento.Disponible = false;
                    _context.Update(asiento);

                    // Crear y guardar el boleto
                    var boletoCreado = _factoryBoleto.CrearBoleto(ruta, categoria, asiento);
                    boletoCreado.Precio = _precioService.CalcularPrecio(boletoCreado);
                    boletoCreado.FechaCompra = DateTime.Now;

                    _context.Add(boletoCreado);
                    await _context.SaveChangesAsync();

                    // Notificar al usuario
                    _notificacionService.EnviarNotificacion($"¡Boleto comprado! Número: {boletoCreado.BoletoID}", this);

                    return RedirectToAction(nameof(Confirmacion), new { id = boletoCreado.BoletoID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                    return await ReloadCreateView(boleto);
                }
            }

            return await ReloadCreateView(boleto);
        }

        private async Task<IActionResult> ReloadCreateView(Boleto boleto)
        {
            ViewBag.Rutas = _context.Rutas.ToList();
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.AsientosDisponibles = await _context.Asientos
                .Where(a => a.Disponible || a.AsientoID == boleto.AsientoID)
                .OrderBy(a => a.NumeroAsiento)
                .ToListAsync();

            return View(boleto);
        }

        // GET: Boletos/Confirmacion/5
        public async Task<IActionResult> Confirmacion(int? id)
        {
            if (id == null) return NotFound();

            var boleto = await _context.Boletos
                .Include(b => b.Ruta)
                .Include(b => b.Categoria)
                .Include(b => b.Asiento)
                .FirstOrDefaultAsync(m => m.BoletoID == id);

            return boleto == null ? NotFound() : View(boleto);
        }

        // GET: Boletos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Boletos
                .Include(b => b.Ruta)
                .Include(b => b.Categoria)
                .Include(b => b.Asiento)
                .OrderByDescending(b => b.FechaCompra)
                .ToListAsync());
        }
    }
}