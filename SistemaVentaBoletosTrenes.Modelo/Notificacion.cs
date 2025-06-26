using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentaBoletosTrenes.Modelo
{
    public class Notificacion
    {
        public int NotificacionID { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }

        // Clave foranea
        public int ClienteID { get; set; }  

        // Relación con Cliente
        public Cliente? Cliente { get; set; }

    }
}
