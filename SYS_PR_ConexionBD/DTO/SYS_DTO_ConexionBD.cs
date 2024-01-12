using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYS_PR_ConexionBD.DTO
{ 
    public class SYS_DTO_ConexionBD
    {
        public string ServidorBD { get; set; }
        public string BaseDatos { get; set; }
        public string UsuarioBD { get; set; }
        public string PasswordBD { get; set; }
        public string TipoBaseDatos { get; set; }
        public string ConexionPorConfianza { get; set; }
        public string UsuarioServiceLayer { get; set; }
        public string PasswordServiceLayer { get; set; }
        public string TimeOutServiceLayer { get; set; }

    }
}
