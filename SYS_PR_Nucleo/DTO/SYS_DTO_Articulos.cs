using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SYS_PR_Nucleo.Clases.DTO
{
    //SE GENERA DTO CON LA ESTRUCUTRA NECESARIA SIMILAR BD
    public class SYS_DTO_Articulos
    {
        public string CodigoFamilia { get; set; }
        public string CodigoBarras { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }

    }


    //SE GENERA DTO PARA MENEJO DE FAMILIAS
    public class SYS_DTO_Familia
    {
        public string Codigo { get; set; }
        public string Nobre { get; set; }
        public int descuento { get; set; }


    }
}
