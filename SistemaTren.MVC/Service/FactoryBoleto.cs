using SistemaVentaBoletosTrenes.Modelo;

namespace SistemaTren.MVC.Service
{
    public class FactoryBoleto
    {
        // Método para crear un nuevo boleto usando el patrón Factory
        public Boleto CrearBoleto(Ruta ruta, Categoria categoria, Asiento asiento)
        {
            return new Boleto
            {
                Ruta = ruta,
                Categoria = categoria,
                Asiento = asiento,
                FechaCompra = DateTime.Now,
                // El precio se calculará posteriormente con PrecioService
                Precio = 0
            };
        }
    }
}