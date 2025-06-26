using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaVentaBoletosTrenes.Modelo;

namespace SistemaTren.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Asiento> Asientos { get; set; } = default!;
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Boleto> Boletos { get; set; } = default!;
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Categoria> Categorias { get; set; } = default!;
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Cliente> Clientes { get; set; } = default!;
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Notificacion> Notificaciones { get; set; } = default!;
        public DbSet<SistemaVentaBoletosTrenes.Modelo.Ruta> Rutas { get; set; } = default!;
    }
}
