//PROYECTO:                      SYS_PR_POSOneAcceso.CS.Ayuda.SYS_CL_Extenciones
//PROPOSITO:                     Clase encargada de funciones de ayuda para la aplicacion
//AUTOR:                         Bernardo Trejo
//FECHA DE CREACION:             05.07.2021
//AUTORES(ES) DE ACTUALIZACION:  
//FECHA DE ULTIMA ACTUALIZACION: 
//OBSERVACIONES:

using Microsoft.AspNetCore.Http;

namespace SYS_PR_Acceso.CS.Ayuda
{
    public static class SYS_CL_Extenciones
    {
        /// <summary>
        /// Metodo que agrega un error a la aplicacion si no se tiene control sobre el
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        public static void AgregarErrorAplicacion(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
