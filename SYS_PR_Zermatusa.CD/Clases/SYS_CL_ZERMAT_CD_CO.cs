
using System;
using System.Collections.Generic;
using SYS_PR_Nucleo.Clases.DTO;
using System.Reflection;
using System.Text;

namespace SYS_PR_Zermat.CD.Clases
{
    public partial class SYS_CL_Zermat_CD
    {

        //SE GENERAN CONSULTAS DIERECTAS EN SQL
        #region Declaracion de Variables
        #endregion
        #region Eventos
        #endregion
        #region Funciones
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string ConsultaObtenerConfiguraciones()
        {              

            StringBuilder oConsulta = new StringBuilder();

            try
            {


                        oConsulta.AppendLine("SELECT ");
                        oConsulta.AppendLine("ISNULL(C.U_SYS_RUTB, '') BITACORATRANSACCIONES,");
                        oConsulta.AppendLine("ISNULL(CI.\"Language\", -1) LENGUAJE");
                        oConsulta.AppendLine("FROM SYS_PCONFIGURACION C");
                        oConsulta.AppendLine("INNER JOIN CINF CI ON (1=1)");
                        oConsulta.AppendLine("WHERE C.Code = 'CON'");


            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermar_CD.ConsultaObtenerConfiguraciones. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <returns>Regresa la consulta para conocer el usuario configurado </returns>
        /// <remarks></remarks>   
        public string ConsultaObtenUsuario(string sUsuario)
        {

            StringBuilder oConsulta = new StringBuilder();

            try
            {


                        oConsulta.AppendLine("SELECT ISNULL(U_SYS_CONT,'') PASS");
                        oConsulta.AppendLine("FROM SYS_PCAJEROS ");
                        oConsulta.AppendFormat("WHERE Code = '{0}' ", sUsuario).AppendLine();


            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaObtenUsuario. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public string ConsultaInsertaVenta(SYS_DTO_Venta oMovimiento)
        {


            StringBuilder oConsulta = new StringBuilder();

            try
            {

                oConsulta.AppendLine("INSERT INTO [ZTRANSACCIONES] (");
                oConsulta.AppendLine("SYS_FOLIO,");
                oConsulta.AppendLine("SYS_SUCURSAL,");
                oConsulta.AppendLine("SYS_TOTAL,");
                oConsulta.AppendLine("SYS_DESCUENTO)");
                oConsulta.AppendLine("VALUES(");
                oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_FOLIO).AppendLine();
                oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_SUCURSAL).AppendLine();
                oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_TOTAL).AppendLine();
                oConsulta.AppendFormat("'{0}')", oMovimiento.SYS_DESCUENTO).AppendLine();


            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaInsertaDetalleVenta. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public string ConsultaInsertaDetalleVenta(SYS_DTO_DetalleVenta oMovimiento)
        {
               

            StringBuilder oConsulta = new StringBuilder();

            try
            {

                        oConsulta.AppendLine("INSERT INTO [ZDETALLETRANS] (");
                        oConsulta.AppendLine("SYS_ARTICULO,");
                        oConsulta.AppendLine("SYS_PRECIO,");
                        oConsulta.AppendLine("SYS_IVA,");
                        oConsulta.AppendLine("SYS_DESCUENTO,");
                        oConsulta.AppendLine("SYS_CANTIDAD,");
                        oConsulta.AppendLine("SYS_TOTAL)");
                        oConsulta.AppendLine("VALUES(");
                        oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_ARTICULO).AppendLine();
                        oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_PRECIO).AppendLine();
                        oConsulta.AppendFormat("'{0}' ,", oMovimiento.SYS_IVA).AppendLine();
                        oConsulta.AppendFormat("'{0}',", oMovimiento.SYS_DESCUENTO).AppendLine();
                        oConsulta.AppendFormat("'{0}',", oMovimiento.SYS_CANTIDAD).AppendLine();
                        oConsulta.AppendFormat("'{0}')", oMovimiento.SYS_TOTAL).AppendLine();

            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaInsertaDetalleVenta. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        /// <remarks></remarks>   
        public string ConsultaDescuentoFamilia(string sCodigo)
        {

            StringBuilder oConsulta = new StringBuilder();

            try
            {
                        oConsulta.AppendLine("SELECT ISNULL(SYS_DESCUENTO, 0) descuento");
                        oConsulta.AppendLine("FROM ZFAMILIA ");
                        oConsulta.AppendFormat("WHERE Code = '{0}' ", sCodigo).AppendLine();

            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaDescuentoFamilia. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        /// <remarks></remarks>   
        public string ConsultaCodigoFamiliaArticulo(string sCodigo)
        {

            StringBuilder oConsulta = new StringBuilder();

            try
            {
                oConsulta.AppendLine("SELECT ISNULL(SYS_FAMILIA, '') CodigoFamilia");
                oConsulta.AppendLine("FROM ZARTICULOS ");
                oConsulta.AppendFormat("WHERE SYS_CODIGO = '{0}' ", sCodigo).AppendLine();

            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaCodigoFamiliaArticulo. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        /// <remarks></remarks>   
        public string ConsultaObtenerArticulos(string sCodigo, bool bTipo)
        {

            StringBuilder oConsulta = new StringBuilder();

            try
            {
                oConsulta.AppendLine("SELECT ISNULL(SYS_FAMILIA, '') CodigoFamilia, SYS_CODBARRAS CodigoBarras, SYS_CODIGO Nombre, SYS_DESC Descripcion, SYS_PRECIO Precio");
                oConsulta.AppendLine("FROM ZARTICULOS ");
                oConsulta.AppendFormat("WHERE {0} = '{1}' ",(bTipo)?"SYS_CODIGO": "SYS_CODBARRAS", sCodigo).AppendLine();

            }
            catch (Exception ex)
            {
                oConsulta = new StringBuilder();
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD_CO.ConsultaCodigoFamiliaArticulo. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConsulta.ToString();

        }


        #endregion
        #region Procedimientos
        #endregion
        #region Propiedades
        #endregion
    }
}
