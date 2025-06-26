using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentaBoletosTrenes.Modelo
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        // Relaciones
        public List<Notificacion>? Notificaciones { get; set; }

    }
}
