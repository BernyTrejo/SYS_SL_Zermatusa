using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SYS_PR_Nucleo.Clases.DTO
{
    public class SYS_DTO_Conexion
    {
        [Required(ErrorMessage = "Configure ServidorBD")]
        public string ServidorBD { get; set; }
        [Required(ErrorMessage = "Configure BaseDatos")]
        public string BaseDatos { get; set; }
        [Required(ErrorMessage = "Configure UsuarioBD")]
        public string UsuarioBD { get; set; }
        [Required(ErrorMessage = "Configure PasswordBD")]
        public string PasswordBD { get; set; }
        [Required(ErrorMessage = "Configure TipoBaseDatos")]
        public string TipoBaseDatos { get; set; }
        [Required(ErrorMessage = "Configure ConexionPorConfianza")]
        public string ConexionPorConfianza { get; set; }

    }
}
