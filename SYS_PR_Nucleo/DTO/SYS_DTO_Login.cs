using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SYS_PR_Nucleo.Clases.DTO
{
    public class SYS_DTO_Login
    {
        [Required(ErrorMessage = "Defina User")]
        public string User { get; set; }
        [Required(ErrorMessage = "Defina Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Defina Sucursal")]
        public string Sucursal { get; set; }
        [Required(ErrorMessage = "Defina Caja")]
        public string Caja { get; set; }
        public string Origen { get; set; }
        public string Token { get; set; }
        public string Vigencia { get; set; }
        public string Descripcion { get; set; }

    }
}
