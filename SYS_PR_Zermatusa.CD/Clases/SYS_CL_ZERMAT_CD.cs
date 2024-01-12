using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading;
using SYS_PR_ConexionBD.Clases;
using SYS_PR_ConexionBD.DTO;
using SYS_PR_Nucleo.Clases;
using SYS_PR_Nucleo.Clases.DTO;

namespace SYS_PR_Zermat.CD.Clases
{
    public partial class SYS_CL_Zermat_CD
    {
        #region Declaracion de Variables
        private SYS_CL_ConexionBD oConexionBD = null;
        
        public enum StatusCodes
        {
            OK = 200,
            BadRequest = 400,
            NotFound = 404,
            InternalError = 500
        };
        #endregion
        #region Eventos
        #endregion
        #region Procedimientos
        /// <summary>
        /// Metodo constructor de la clase
        /// </summary>
        /// <param name="oDatosConexion"></param>
        /// <param name="sInstancia"></param>
        /// <param name="oConfiguraciones"></param>
        public SYS_CL_Zermat_CD(SYS_DTO_Conexion oDatosConexion, ref SYS_DTO_Configuraciones oConfiguraciones)
        {
               
            //se configura la conexion de BD
            SYS_PR_ConexionBD.DTO.SYS_DTO_ConexionBD oDatosConexionBD = null;
            string sConsulta = "";
            DataSet oDatos = null;
            DataRow oFila = null;

            try
            {

                if (Thread.CurrentThread.CurrentCulture.Name != "es-MX")
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-MX", false);

                    if (System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator != ".")
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
                        System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator = ",";
                    }

                    if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                        System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = ",";
                    }
                }

                //se instancian parametros para mandar a llamar al proyecto de conexiones
                if (oDatosConexion == null)
                    throw new System.ArgumentException("Existe un error en las configuraciones de conexion de la aplicacion Zermar");

                oDatosConexionBD = new SYS_DTO_ConexionBD();
                oDatosConexionBD.ServidorBD = oDatosConexion.ServidorBD;
                oDatosConexionBD.BaseDatos = oDatosConexion.BaseDatos;
                oDatosConexionBD.UsuarioBD = oDatosConexion.UsuarioBD;
                oDatosConexionBD.PasswordBD = oDatosConexion.PasswordBD;
                oDatosConexionBD.TipoBaseDatos = oDatosConexion.TipoBaseDatos;
                oDatosConexionBD.ConexionPorConfianza = oDatosConexion.ConexionPorConfianza;

                oConexionBD = new SYS_CL_ConexionBD(oDatosConexionBD);

                if (oConexionBD == null)
                {
                    throw new System.ArgumentException("No se logro conectar a la base de datos");
                }

                //se consultan las configuraciones
                sConsulta = ConsultaObtenerConfiguraciones();
                oDatos = oConexionBD.ObtenerDatos(sConsulta, "CONFIGURACIONES");

                if (oDatos == null || oDatos.Tables[0].Rows.Count == 0 || oDatos.Tables[0].Rows.Count > 1)
                {
                    throw new System.ArgumentException("No se lograron obtener las configuraciones generales");
                }

                //se guarda configuracion por medio de un DTO
                oConfiguraciones = new SYS_DTO_Configuraciones();

                oFila = oDatos.Tables[0].Rows[0];

                if (Convert.ToInt32(oFila["LENGUAJE"]) == -1 || //No esta definido
                    Convert.ToInt32(oFila["LENGUAJE"]) == 2 || //BoSuppLangs.ln_Spanish_Ar
                    Convert.ToInt32(oFila["LENGUAJE"]) == 7 || //BoSuppLangs.ln_Spanish_Pa
                    Convert.ToInt32(oFila["LENGUAJE"]) == 23 || //BoSuppLangs.ln_Spanish
                    Convert.ToInt32(oFila["LENGUAJE"]) == 25) //BoSuppLangs.ln_Spanish_La
                    oConfiguraciones.LenguajeLocal = true;
                else
                    oConfiguraciones.LenguajeLocal = false;

                oConexionBD.bLenguajeLocal = oConfiguraciones.LenguajeLocal;
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_Zermat_CD.New. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

        }

        #endregion
        #region Funciones
        /// <summary>
        /// Función que obtiene el usuario y la contraseña
        /// </summary>
        /// <param name="oLogin"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool ExisteUsuario(SYS_DTO_Login oLogin, ref string sError, ref int iCode)
        {


            DataSet oDatos = null;
            DataRow oFila = null;
            string sConsuta = "";
            bool oResultado = false;
            string sPassDesc = "";

            try
            {
                //se consulta  datos para el usuario

                sConsuta = ConsultaObtenUsuario(oLogin.User);
                oDatos = oConexionBD.ObtenerDatos(sConsuta, "USUARIO"); //se obtiene datos y se guardan en un dataset

                if (oDatos == null || oDatos.Tables[0].Rows.Count != 1) //se valida dataset que tenga datos
                {
                    sError = "No existe el usuario en la BD";
                    iCode = Convert.ToInt32(StatusCodes.NotFound);
                    return false;
                }

                //se obtiene informacion del usuario
                oFila = oDatos.Tables["USUARIO"].Rows[0]; 


                sPassDesc = Convert.ToString(oFila["PASS"]).Trim();

                if (sPassDesc == oLogin.Password.Trim())
                {
                    oResultado = true;
                }
                else//se maneja error por medio de paremtro de retorno
                { 
                    sError = "La contraseña no coincide con la obtenida en el servidor";
                    iCode = Convert.ToInt32(StatusCodes.NotFound);
                }

            }
            catch (Exception ex)
            {
                iCode = Convert.ToInt32(StatusCodes.BadRequest);
                sError = ex.Message;
                oResultado = false;
            }

            return oResultado;

        }

        /// <summary>
        /// Función que obtiene el usuario y la contraseña
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool ConfiguraVenta(ref SYS_DTO_Venta oMovimiento, ref string sError, ref int iCode)
        {

            DataSet oDatos = null;
            DataRow oFila = null;
            string sConsuta = "";
            bool oResultado = false;
            double dDescuento = 0;
            double dTotal = 0;
            try
            {

                //por medio de un for recorremos todos los articulos de la venta
                for (int i = 0; i < oMovimiento.detalle.Count - 1; i++)
                {
                    dTotal = 0;


                    //se obtiene codigo de familia perteneciente al articulo
                    
                    sConsuta = ConsultaCodigoFamiliaArticulo(oMovimiento.detalle[i].SYS_ARTICULO);
                    oDatos = oConexionBD.ObtenerDatos(sConsuta, "FAMILIA");

                    if (oDatos == null || oDatos.Tables[0].Rows.Count != 1)
                    {
                        sError = "Error al obtener Codigo de familia del articulo";
                        iCode = Convert.ToInt32(StatusCodes.NotFound);
                        return false;
                    }

                    oFila = oDatos.Tables["FAMILIA"].Rows[0];

                    //se poobtiene descuento de familia de articulo
                    sConsuta = ConsultaDescuentoFamilia(Convert.ToString(oFila["CodigoFamilia"]));
                    oDatos = oConexionBD.ObtenerDatos(sConsuta, "Descuento");

                    if (oDatos == null || oDatos.Tables[0].Rows.Count != 1)
                    {
                        sError = "Error al obtener descuento";
                        iCode = Convert.ToInt32(StatusCodes.NotFound);
                        return false;
                    }

                    oFila = oDatos.Tables["Descuento"].Rows[0];

                    dDescuento = Convert.ToDouble(oFila["descuento"]);

                    //se obtiene impuesto 
                    dTotal = (oMovimiento.detalle[i].SYS_PRECIO * oMovimiento.detalle[i].SYS_IVA);

                    //se suma el impuesto y se le aplica la cantidad
                    dTotal = ((dTotal + oMovimiento.detalle[i].SYS_PRECIO) + oMovimiento.detalle[i].SYS_CANTIDAD);

                    //si es diferente de 0 se aplica el descuento
                    if (dDescuento != 0)
                    {
                        dTotal = dTotal - (dTotal * dDescuento);
                    }

                    //se guarda informacion en el dto de los calculos
                    oMovimiento.detalle[i].SYS_TOTAL = dTotal;
                    oMovimiento.detalle[i].SYS_DESCUENTO = dDescuento;

                }

            }
            catch (Exception ex)
            {
                iCode = Convert.ToInt32(StatusCodes.BadRequest);
                sError = ex.Message;
                oResultado = false;
            }

            return oResultado;

        }

        /// <summary>
        /// </summary>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool IntertarRegistroDetalleTransaccion(SYS_DTO_Venta oMovimiento, ref string sError, ref int iCode, ref IDbConnection oConexion, ref IDbCommand oComando, ref IDbTransaction oTransaccion)
        {
             

            bool bResultado = false;
            string[] sConsulta = null;

            try
            {
                //se inserta registro de la transaccion perteneciente al encabezado
                sConsulta = new string[1];
                sConsulta[0] = ConsultaInsertaVenta(oMovimiento);
                if (!oConexionBD.AfectarBD(sConsulta, ref oConexion, ref oComando, ref oTransaccion, ref sError))
                {
                    sError = "Error al crear el registro de transaccion";
                    if (oTransaccion != null)
                    {
                        oTransaccion.Rollback();//se maneja rollback para no guardar proceso de transaccion
                        oTransaccion.Dispose();
                        oTransaccion = null;
                    }
                    iCode = 500;
                    return false;
                }

                foreach (var item in oMovimiento.detalle)//se recorre los detalles de la transaccion para hacer insert en BD
                {

                    sConsulta = new string[1];
                    sConsulta[0] = ConsultaInsertaDetalleVenta(item);
                    if (!oConexionBD.AfectarBD(sConsulta, ref oConexion, ref oComando, ref oTransaccion, ref sError))
                    {
                        sError = "Error al crear el registro de transaccion";
                        if (oTransaccion != null)
                        {
                            oTransaccion.Rollback();
                            oTransaccion.Dispose();
                            oTransaccion = null;
                        }
                        iCode = 500;
                        return false;
                    }
                }


                bResultado = true;
            }
            catch (Exception ex)
            {
                iCode = 500;
                sError = ex.Message;
                bResultado = false;
            }

            return bResultado;

        }

        /// <summary>
        /// </summary>
        /// <param name="oArticulos"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool ObtenerArticulo(ref SYS_DTO_Articulos oArticulo, ref string sError, ref int iCode)
        {

            DataSet oDatos;
            string oConsulta;
            bool bResultado;
            DataRow oFila = null;

            try
            {
                //se valida codigo de barras y si esta limpio se valida el nombre del articulo
                if (oArticulo.CodigoBarras != null && oArticulo.CodigoBarras != "")
                    oConsulta = ConsultaObtenerArticulos(oArticulo.CodigoBarras, false);
                else
                    oConsulta = ConsultaObtenerArticulos(oArticulo.Nombre, true);

                oDatos = oConexionBD.ObtenerDatos(oConsulta, "ARTICULO"); //se obtiene informacion del articulo en dataset

                if (oDatos == null || oDatos.Tables[0].Rows.Count == 0)
                {
                    sError = "No se encontraron registros de artículo";
                    return false;
                }

                oFila = oDatos.Tables[0].Rows[0];
                
                //se gurdan los datos del articulo en DTO
                oArticulo.CodigoFamilia = oFila["CodigoFamilia"].ToString();
                oArticulo.CodigoBarras = oFila["CodigoBarras"].ToString();
                oArticulo.Nombre = oFila["Nombre"].ToString();
                oArticulo.Descripcion = oFila["Descripcion"].ToString();
                oArticulo.Precio = Convert.ToDouble(oFila["Precio"]);

                bResultado = true;

            }
            catch (Exception ex)
            {
                iCode = Convert.ToInt32(StatusCodes.BadRequest);
                sError = ex.Message;
                bResultado = false;
            }

            return bResultado;

        }
        #endregion
        #region Propiedades
        #endregion
    }
}
