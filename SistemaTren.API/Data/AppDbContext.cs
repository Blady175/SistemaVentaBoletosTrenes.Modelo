using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVentaBoletosTrenes.Modelo;

    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
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
