using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentaBoletosTrenes.Modelo
{
    public class Boleto
    {
        public int BoletoID { get; set; }
        public double Precio { get; set; }
        public DateTime FechaCompra { get; set; }

        //Claves foraneas
        public int RutaID { get; set; }  
        public int CategoriaID { get; set; }  
        public int AsientoID { get; set; }

        public int NumeroAsiento { get; set; }

        // Relaciones
        public Ruta? Ruta { get; set; }
        public Categoria? Categoria { get; set; }
        public Asiento? Asiento { get; set; }

    }
}
