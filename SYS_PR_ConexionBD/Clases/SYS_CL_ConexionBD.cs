
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using SYS_PR_ConexionBD.DTO;
using System.Linq;

namespace SYS_PR_ConexionBD.Clases
{
    public partial class SYS_CL_ConexionBD
    {
        #region Declaración de Variables
        private string sCadenaConexion = "";
        public eTipoBaseDatos oTipoServidorBD;
        private Object oBloqueoCodigoTrans = new Object();
        public bool bLenguajeLocal = true;

        #endregion

        #region Eventos
        public enum eTipoBaseDatos
        {
            SQL = 1,
        };

        #endregion

        #region Procedimientos
        /// <summary>
        /// </summary>
        /// <param name="oDatosConexion"></param>
        /// <param name="sInstancia"></param>
        /// <param name="oConfiguraciones"></param>
        public SYS_CL_ConexionBD(SYS_DTO_ConexionBD oDatosConexion)
        {


            string sPassWord = "";
            string sError = "";

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

                if(oDatosConexion == null )
                    throw new System.ArgumentException("Existe un error en las configuraciones de conexion de la aplicacion");
                if (oDatosConexion.ServidorBD.Trim() == "")
                    throw new System.ArgumentException("No se tiene definido el servidor de la base de datos");
                if (oDatosConexion.BaseDatos.Trim() == "")
                    throw new System.ArgumentException("No se tiene definida la base de datos");                                
                if (oDatosConexion.TipoBaseDatos.Trim() == "")
                    throw new System.ArgumentException("No se tiene definido el tipo de servidor de base de datos");
                if (oDatosConexion.ConexionPorConfianza.Trim() != "Y")
                {
                    if (oDatosConexion.UsuarioBD.Trim() == "")
                        throw new System.ArgumentException("No se tiene definido el usuario de base de datos");

                    if (oDatosConexion.PasswordBD.Trim() == "")
                        throw new System.ArgumentException("No se tiene definida la contraseña del usuario de base de datos");

                    sPassWord = oDatosConexion.PasswordBD;

                    if (sPassWord.Trim() == "")
                        throw new System.ArgumentException("No se logro desencriptar la contraseña del usuario de base de datos");
                }


              
                    if (oDatosConexion.ConexionPorConfianza.Trim() == "Y")
                        sCadenaConexion = String.Format("Data Source = {0};Integrated Security = True; Initial Catalog = {1}", oDatosConexion.ServidorBD.Trim(), oDatosConexion.BaseDatos.Trim());
                    else
                        sCadenaConexion = String.Format("Data Source = {0};Initial Catalog = {1}; user id = {2};password = {3}", oDatosConexion.ServidorBD.Trim(), oDatosConexion.BaseDatos.Trim(), oDatosConexion.UsuarioBD.Trim(), sPassWord.Trim());


                if (!ConectarBD(ref sError, sCadenaConexion))
                {
                    if(sError.Trim() == "")
                        throw new System.ArgumentException("No se logro obtener comunicacion con la base de datos");
                    else
                        throw new System.ArgumentException(sError);
                }

            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.New. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="oDatos"></param>
        /// <param name="sConsulta"></param>
        /// <param name="sNombreTabla"></param>
        public void ObtenerDatos(string sConsulta, string sNombreTabla, ref DataSet oDatos)
        {
            //FUNCION ENCARGADA DE CREAR EL ADAPTADOR Y HACER CONSULTAS DIRECTAS A BD REGRESA UN DATASET

            IDbConnection oConexion = null;
            IDbDataAdapter oAdaptador = null;
            SqlDataAdapter oAdaptadorSQL = null;

            try
            {

                oConexion = CrearObjetoConexion(sCadenaConexion);

                if (oDatos == null)
                    oDatos = new DataSet();

                oAdaptador = CrearObjetoDataAdapterConsulta(sConsulta, oConexion);
                oAdaptador.SelectCommand.CommandTimeout = 0;


                    oAdaptadorSQL = (SqlDataAdapter)oAdaptador;
                    oAdaptadorSQL.Fill(oDatos, sNombreTabla);


            }
            catch (Exception ex)
            {
                oDatos = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.ObtenerDatosRef. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }
            finally
            {
                oAdaptador = null;
                oConexion = null;
            }

        }

        #endregion

        #region Funciones

 
        /// <summary>
        /// </summary>
        /// <param name="sConsulta"></param>
        /// <param name="sNombreTabla"></param>
        /// <returns></returns>
        public DataSet ObtenerDatos(string sConsulta, string sNombreTabla)
        {
            //OBTIENE UN DATASE DIRECTO DE BD AGREANDO UN NOMBRE ESPECIFICO DE TABLA
            DataSet oDatos = null;
            IDbConnection oConexion = null;
            IDbDataAdapter oAdaptador = null;

            try
            {

                oConexion = CrearObjetoConexion(sCadenaConexion);
                oDatos = new DataSet();
                oAdaptador = CrearObjetoDataAdapterConsulta(sConsulta, oConexion);
                oAdaptador.SelectCommand.CommandTimeout = 0;

                oAdaptador.Fill(oDatos);

                oDatos.Tables[0].TableName = sNombreTabla;

            }
            catch (Exception ex)
            {
                oDatos = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.ObtenerDatos. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }
            finally
            {
                oAdaptador = null;
                oConexion = null;
            }

            return oDatos;

        }

        /// <summary>
        /// </summary>
        /// <param name="sConsulta"></param>
        /// <param name="sNombreTabla"></param>
        /// <param name="oComando"></param>
        /// <returns></returns>
        public DataSet ObtenerDatos(string sConsulta, string sNombreTabla, ref IDbCommand oComando)
        {

            DataSet oDatos = null;
            
            IDbDataAdapter oAdaptador = null;

            try
            {

                //OBTIENE DATOS CUANDO UNA CONEXION O TRANSACCION ESTA VIVA
                oDatos = new DataSet();
                oComando.Parameters.Clear();
                oComando.CommandText = sConsulta;                
                oAdaptador = CrearObjetoDataAdapterComando(ref oComando);
                oAdaptador.SelectCommand.CommandTimeout = 0;

                oAdaptador.Fill(oDatos);

                oDatos.Tables[0].TableName = sNombreTabla;

            }
            catch (Exception ex)
            {
                oDatos = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.ObtenerDatosTransaccion. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }
            finally
            {
                oAdaptador = null;                
            }

            return oDatos;

        }

        /// <summary>
        /// </summary>
        /// <param name="sCadenaConexion"></param>
        /// <returns></returns>
        public IDbConnection CrearObjetoConexion(string sCadenaConexion)
        {

            IDbConnection oConexion = null;

            try
            {

                        oConexion = new SqlConnection(sCadenaConexion);

            }
            catch (Exception ex)
            {
                oConexion = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.CrearObjetoConexion. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oConexion;

        }

        /// <summary>
        /// </summary>
        /// <param name="sConsulta"></param>
        /// <param name="oConexion"></param>
        /// <returns></returns>
        public IDbDataAdapter CrearObjetoDataAdapterConsulta(string sConsulta, IDbConnection oConexion)
        {

            IDbDataAdapter oAdaptador = null;

            try
            {
                        oAdaptador = new SqlDataAdapter(sConsulta, (SqlConnection)oConexion);
            }
            catch (Exception ex)
            {
                oAdaptador = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.CrearObjetoDataAdapterConsulta. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oAdaptador;

        }

        /// <summary>
        /// </summary>
        /// <param name="oComando"></param>
        /// <returns></returns>
        public IDbDataAdapter CrearObjetoDataAdapterComando(ref IDbCommand oComando)
        {


            IDbDataAdapter oAdaptador = null;

            try
            {

                        oAdaptador = new SqlDataAdapter((SqlCommand)oComando);

            }
            catch (Exception ex)
            {
                oAdaptador = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.CrearObjetoDataAdapterComando. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oAdaptador;

        }

        /// <summary>
        /// </summary>
        /// <param name="sConsulta"></param>
        /// <param name="oConexion"></param>
        /// <param name="oComando"></param>
        /// <param name="oTransaccion"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public bool AfectarBD(string[] sConsulta, ref IDbCommand oComando, ref string sError)
        {


            bool bResultado = false;
            IDbConnection oConexion = null;
            IDbTransaction oTransaccion = null;

            try
            {
                if (oComando == null)
                {
                    oConexion = CrearObjetoConexion(sCadenaConexion);

                    if (oConexion.State == ConnectionState.Open)
                        oConexion.Close();

                    oConexion.Open();

                    oComando = CrearObjetoComando(oConexion);

                    oTransaccion = CrearObjetoTransaccion(oConexion);
                    oComando.Transaction = oTransaccion;
                }
                  
                for (int iContador = 0; iContador < sConsulta.Length; iContador++)
                    if (sConsulta[iContador] != null && sConsulta[iContador].Trim() != "")
                    {
                        oComando.CommandText = sConsulta[iContador];
                        oComando.ExecuteNonQuery();
                    }

                bResultado = true;
                
            }
            catch (Exception ex)
            {
                bResultado = false;
                if (oTransaccion != null)
                {
                    oTransaccion.Rollback();
                    oTransaccion.Dispose();
                }
                sError = String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.AfectarBD. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message);
            }
            
            return bResultado;

        }

        public bool AfectarBD(string[] sConsulta, ref IDbConnection oConexion, ref IDbCommand oComando, ref IDbTransaction oTransaccion, ref string sError)
        {

            bool bResultado = false;
            
            try
            {
                if (oComando == null)
                {
                    oConexion = CrearObjetoConexion(sCadenaConexion);

                    if (oConexion.State == ConnectionState.Open)
                        oConexion.Close();

                    oConexion.Open();

                    oComando = CrearObjetoComando(oConexion);

                    oTransaccion = CrearObjetoTransaccion(oConexion);
                    oComando.Transaction = oTransaccion;
                }

                if (oTransaccion == null)
                {
                    oTransaccion = CrearObjetoTransaccion(oConexion);
                    oComando.Transaction = oTransaccion;
                }

                for (int iContador = 0; iContador < sConsulta.Length; iContador++)
                    if (sConsulta[iContador] != null && sConsulta[iContador].Trim() != "")
                    {
                        oComando.CommandText = sConsulta[iContador];
                        oComando.ExecuteNonQuery();
                    }

                bResultado = true;

            }
            catch (Exception ex)
            {
                bResultado = false;
                if (oTransaccion != null)
                {
                    oTransaccion.Rollback();
                    oTransaccion.Dispose();
                    oTransaccion = null;
                }
                sError = String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.AfectarBD. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message);
            }

            return bResultado;

        }

        /// <summary>
        /// </summary>
        /// <param name="sConsulta"></param>
        /// <returns></returns>
        public bool AfectarBD(string[] sConsulta, ref string sError)
        {


            IDbConnection oConexion = null;
            IDbCommand oComando = null;
            IDbTransaction oTransaccion = null;

            bool bResultado = false;

            try
            {
                oConexion = CrearObjetoConexion(sCadenaConexion);

                if (oConexion.State == ConnectionState.Open)
                    oConexion.Close();

                oConexion.Open();

                oComando = CrearObjetoComando(oConexion);

                oTransaccion = CrearObjetoTransaccion(oConexion);
                oComando.Transaction = oTransaccion;

                for (int iContador = 0; iContador < sConsulta.Length; iContador++)
                    if (sConsulta[iContador] != null && sConsulta[iContador].Trim() != "")
                    {
                        oComando.CommandText = sConsulta[iContador];
                        oComando.ExecuteNonQuery();
                    }

                oTransaccion.Commit();
                oTransaccion.Dispose();
                
                bResultado = true;

            }
            catch (Exception ex)
            {
                bResultado = false;
                if (oTransaccion != null)
                {
                    oTransaccion.Rollback();
                    oTransaccion.Dispose();
                }
                sError = ex.Message;                
            }
            finally
            {
                if (oConexion != null && oConexion.State == ConnectionState.Open)
                    oConexion.Close();
                oConexion.Dispose();
                oConexion = null;
                oComando = null;
                oConexion = null;
                oTransaccion = null;
            }

            return bResultado;

        }

        /// <summary>
        /// </summary>
        /// <param name="oConexion"></param>
        /// <returns></returns>
        public IDbTransaction CrearObjetoTransaccion(IDbConnection oConexion)
        {


            IDbTransaction oTransaccion = null;
            try
            {
                        oTransaccion = ((SqlConnection)oConexion).BeginTransaction();
            }
            catch (Exception ex)
            {
                oTransaccion = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.CrearObjetoTransaccion. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));               
            }

            return oTransaccion;

        }

        /// <summary>
        /// </summary>
        /// <param name="oConexion"></param>
        /// <returns></returns>
        public IDbCommand CrearObjetoComando(IDbConnection oConexion)
        {

            IDbCommand oComando = null;

            try
            {

                        oComando = ((SqlConnection)oConexion).CreateCommand();

            }
            catch (Exception ex)
            {
                oComando = null;
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.CrearObjetoComando. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return oComando;

        }

        public bool ConectarBD(ref string sMensaje, string sCadenaConexion)
        {
              

            bool bResultado = false;
            IDbConnection oConexion = null;
            int ierror = 0;

            try
            {
   
                        try
                        {
                            oConexion = CrearObjetoConexion(sCadenaConexion);
                            oConexion.Open();
                        }
                        catch (SqlException ex)
                        {
                            ierror = ex.Number;
                            switch (ierror)
                            {
                                case 4060:
                                    sMensaje = "La base de datos no existe";
                                    break;
                                case 18456:
                                    sMensaje = "La contraseña es incorrecta o el usuario no existe";
                                    break;
                                case -1:
                                    sMensaje = "No es correcto el servidor";
                                    break;
                                case 2:
                                    sMensaje = "No es correcto el servidor";
                                    break;
                                default:
                                    sMensaje = String.Format("No se puede realizar la conexion a la BD por el siguietne error: {0}", ex.Message);
                                    break;
                            }
                            return false;
                        }


                if (oConexion != null && oConexion.State == ConnectionState.Open)
                {
                    bResultado = true;
                    oConexion.Close();
                }

            }
            catch (Exception ex)
            {
                sCadenaConexion = "";
                bResultado = false;
                sMensaje = string.Format("Error: {0}", ex.Message);
            }
            finally
            {
                oConexion = null;
            }
            GC.Collect();
            return bResultado;

        }




        /// <summary>
        /// </summary>
        /// <param name="oCodigos">lista de string que contiene los codigos</param>
        /// <returns></returns>
        public string RegresarCondicion(List<string> oCodigos)
        {


            String RegresarCondicion = "";

            try
            {
                var oListaCodigos = (from Ejecucion in oCodigos
                                     select new
                                     {
                                         Codigo = Ejecucion
                                     }
                          ).ToList();

                RegresarCondicion = String.Format("IN ({0}) ", String.Join(",", oListaCodigos.Select(X => String.Format("'{0}'", X.Codigo)).ToList()));
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Ensamblado {0}, Modulo: SYS_CL_ConexionBD.RegresarCondicion. Error:{2} {1}", Assembly.GetExecutingAssembly().GetName().Name, Environment.NewLine, ex.Message));
            }

            return RegresarCondicion;
        }



        #endregion

        #region Propiedades
        #endregion
    }
}
