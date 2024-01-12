using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SYS_PR_Nucleo.Clases.DTO
{
    public class SYS_DTO_Venta
    {

        public string SYS_FOLIO { get; set; }
        public string SYS_SUCURSAL { get; set; }
        public double SYS_TOTAL { get; set; }
        public double SYS_DESCUENTO { get; set; }
        public List<SYS_DTO_DetalleVenta> detalle { get; set; }

    }
    public class SYS_DTO_DetalleVenta
    {
        public string SYS_ARTICULO { get; set; }
        public double SYS_PRECIO { get; set; }
        public double SYS_IVA { get; set; }
        public double SYS_DESCUENTO { get; set; }
        public int SYS_CANTIDAD { get; set; }
        public double SYS_TOTAL { get; set; }
       
    }

}
