using SistemaVentaBoletosTrenes.Modelo;

namespace SistemaTren.MVC.Service
{
    public class PrecioService
    {
        // Precio base para un adulto economico
        private const double PrecioBase = 20.0;

        public double CalcularPrecio(Boleto boleto)
        {
            double precioFinal = PrecioBase;

            // Aplicar descuento según categoría
            switch (boleto.Categoria?.Nombre.ToLower())
            {
                case "niño":
                case "tercera edad":
                    precioFinal *= (1 - boleto.Categoria.Descuento);
                    break;
                case "adulto":
                    // Adultos pagan precio completo
                    break;
                default:
                    // Si la categoría no es reconocida, usar precio base
                    break;
            }

            // Aplicar recargo por asiento preferencial
            if (boleto.Asiento?.TipoAsiento?.ToLower() == "preferencial")
            {
                precioFinal += 5.0; 
            }

            return precioFinal;
        }
    }
}