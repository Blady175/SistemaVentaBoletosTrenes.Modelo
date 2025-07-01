using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTren.MVC.Models;

namespace SistemaVentaBoletosTrenes.Modelo
{
    public class Asiento
    {
        public int AsientoID { get; set; } 
        public string TipoAsiento { get; set; }
        public int NumeroAsiento { get; set; }
        public bool Disponible { get; set; } 
        public List<Boleto>? Boletos { get; set; }
        
    }
}
