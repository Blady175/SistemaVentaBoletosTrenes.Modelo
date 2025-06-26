using Microsoft.AspNetCore.Mvc;

namespace SistemaTren.MVC.Service
{
    public class NotificacionService
    {
        public void EnviarNotificacion(string mensaje, Controller controller)
        {
            //Almacenamos el mensaje y mostramos en la vista
            controller.TempData["Mensaje"] = mensaje;
        }
    }
}
