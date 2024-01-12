using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using SYS_PR_Nucleo.Clases.DTO;
using SYS_PR_Nucleo.Clases;
using SYS_PR_Zermat.CD.Clases;
using System.Data;

namespace SYS_PR_Zermat.CN.Clases
{
    public class SYS_CL_ZERMAT_CN
    {
        #region Declaracion de Variables
        private static SYS_CL_Zermat_CD oCapaDatos = null;
        private static string sUbicacionAplicacion = AppDomain.CurrentDomain.BaseDirectory;
        private static Object oBloqueoLog = new Object();
        private static SYS_DTO_Configuraciones oConfiguraciones = null;
        /// <summary>
        /// </summary>
        /// <remarks></remarks>

        //se crea enumerador para manejar el codigo de respuesta
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
        /// </summary>
        /// <remarks></remarks>
        public SYS_CL_ZERMAT_CN(SYS_DTO_Conexion oDatosConexion)//proceso para instanciar capa de datos y obtener configuraciones
        {            

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

                oCapaDatos = new SYS_CL_Zermat_CD(oDatosConexion, ref oConfiguraciones);

            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_POSOneAcceso_CN.New. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="oLogin"></param>
        /// <param name="sTokenSemilla"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public SecurityToken ObtenerTokenUsuario(ref SYS_DTO_Login oLogin, string sTokenSemilla, string sVigencia, ref string sError, ref int iCode)
        {
  

            SecurityToken token = null;
            int iVigencia;
            try
            {
                //se valida si los parametros estan llenos

                if (oLogin.User == null) {
                    sError = "Defina User";
                    iCode = Convert.ToInt32(StatusCodes.BadRequest);
                    return null;
                }
                if (oLogin.Password == null) {
                    sError = "Defina Password";
                    iCode = Convert.ToInt32(StatusCodes.BadRequest);
                    return null;
                }
                if (oLogin.Sucursal == null) {
                    sError = "Defina Sucursal";
                    iCode = Convert.ToInt32(StatusCodes.BadRequest);
                    return null;
                }
                if (oLogin.Caja == null)
                {
                    sError = "Defina Caja";
                    iCode = Convert.ToInt32(StatusCodes.BadRequest);
                    return null;
                }

                //se valida si existe usuario
                if (!oCapaDatos.ExisteUsuario(oLogin, ref sError, ref iCode))
                {
                    if (sError.Trim() == "")
                        sError = "Ocurrio un error al validar Usuario";
                        iCode = Convert.ToInt32(StatusCodes.InternalError);
                    return null;
                }
                var claims = new[]//se generan parametros para encriptacion de token
                {
                    new Claim(ClaimTypes.NameIdentifier, oLogin.User.Trim()),
                    new Claim(ClaimTypes.Name, oLogin.Sucursal.Trim()),
                    new Claim(ClaimTypes.Country, oLogin.Caja),
                };

                //Generación de token
                //se inicia encriptando la semilla obtenida desde configuraciones appsetings
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sTokenSemilla));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                int.TryParse(sVigencia, out iVigencia);

                //se le agrega un tiempo de vigencia
                oLogin.Vigencia = string.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now.AddDays(iVigencia));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(iVigencia),
                    SigningCredentials = credenciales
                };

                //se crea el token
                var tokenHandler = new JwtSecurityTokenHandler();
                token = tokenHandler.CreateToken(tokenDescriptor);

            }
            catch (Exception ex)
            {
                token = null;
                sError = ex.Message;
                
            }

            return token;

        }


        /// <summary>
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool GenerarMovimiento(ref SYS_DTO_Venta oMovimiento, string sTipoMovimiento, ref string sError, ref int iCode)
        {


            try
            {

                if (oMovimiento.SYS_FOLIO == null)
                {
                    sError = "Defina folio";
                    iCode = Convert.ToInt32(StatusCodes.BadRequest);
                    return false;
                }
            
                //se configura la venta

                if (!oCapaDatos.ConfiguraVenta(ref oMovimiento, ref sError, ref iCode))
                {
                    if (sError.Trim() == "")
                        sError = "Ocurrio un error al configurar la venta";
                    iCode = Convert.ToInt32(StatusCodes.InternalError);
                    return false;
                }


                //genera insert venta
                if (!InsertarMovimiento( oMovimiento, ref sError, ref iCode))
                {
                    if (sError.Trim() == "")
                        sError = "Ocurrio un error al insertar la venta";
                    iCode = Convert.ToInt32(StatusCodes.InternalError);
                    return false;
                }

            }
            catch (Exception ex)
            {
                sError = ex.Message;

            }

            return true;

        }

        /// <summary>
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool InsertarMovimiento(SYS_DTO_Venta oMovimiento, ref string sError, ref int iCode)
        {            

            bool bResultado = false;
            IDbConnection oConexion = null;
            IDbCommand oComando = null;
            IDbTransaction oTransaccion = null;
            string[] sAfectaciones = null;
            SYS_CL_Nucleo oNucleo;
            List<string> oTablasExcluidas = null;
            try
            {

                oTablasExcluidas = new List<string>();

                if (!oCapaDatos.IntertarRegistroDetalleTransaccion(oMovimiento, ref sError, ref iCode, ref oConexion, ref oComando, ref oTransaccion))
                {
                    if (sError.Trim() == "")
                        sError = "No se logro insertar el registro de venta";
                    if (oTransaccion != null)
                    {
                        oTransaccion.Rollback();
                        oTransaccion.Dispose();
                        oTransaccion = null;
                    }
                    iCode = 500;
                    return false;
                }

                oTransaccion.Commit();

                oTransaccion.Dispose();
                oTransaccion = null;
                oConexion.Close();
                oConexion.Dispose();
                oConexion = null;

                bResultado = true;
            }
            catch (Exception ex)

            {
                bResultado = false;
                sError = ex.Message;
                
            }

            return bResultado;

        }


        /// <summary>
        /// </summary>
        /// <param name="oArticulos"></param>
        /// <param name="sError"></param>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public bool ObtieneArticulos(ref SYS_DTO_Articulos oArticulos, ref string sError, ref int iCode)
        {


            try
            {

                if (oArticulos.CodigoBarras == null && oArticulos.CodigoBarras == "")
                {

                    if (oArticulos.Nombre == null && oArticulos.Nombre == "")
                    {
                        sError = "Defina Codigo de barras o Nombre de articulo";
                        iCode = Convert.ToInt32(StatusCodes.BadRequest);
                        return false;
                    }
                }

                if (oArticulos.Nombre == null && oArticulos.Nombre == "")
                {
                    if (oArticulos.CodigoBarras == null && oArticulos.CodigoBarras == "")

                    {
                        sError = "Defina Codigo de barras o Nombre de articulo";
                        iCode = Convert.ToInt32(StatusCodes.BadRequest);
                        return false;
                    }
                }

                //se configura la venta

                if (!oCapaDatos.ObtenerArticulo(ref oArticulos, ref sError, ref iCode))
                {
                    if (sError.Trim() == "")
                        sError = "Ocurrio un error al configurar la venta";
                    iCode = Convert.ToInt32(StatusCodes.InternalError);
                    return false;
                }

            }
            catch (Exception ex)
            {
                sError = ex.Message;

            }

            return true;

        }
        #endregion
        #region Propiedades
        #endregion
    }
}
