
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Dynamic;
using System.Linq;
using SYS_PR_ConexionBD.Clases;
using System.Reflection;
using System.Security.Cryptography;
using SYS_PR_Nucleo.Clases.DTO;

namespace SYS_PR_Nucleo.Clases
{

    //FUNCIONES O METODOS NO UTILIZADOS SE PENSABAN USASR PARA GENERACION DE DATASETS A DTO O INSERTS AUTOMATICOS
    public class SYS_CL_Nucleo
    {
        #region Declaración de Variables   
        List<string> oTablasExcluidas = null;

        #endregion

        #region Eventos
        #endregion

        #region Procedimientos
        #endregion

        #region Funciones

        //CONVERSION DE DATATABLE A OBJETO
        public static List<object> DataTableAObjeto(ref string sError, DataTable oTabla, Type oTipo)
        {


            object oObjetoTabla, oAgregar;
            object[] oResultado = null;
            PropertyInfo oPropiedad;
            List<object> oFinal = null;
            try
            {
                foreach (DataRow oFila in oTabla.Rows)
                {
                    if (oResultado is null)
                    {
                        oResultado = new object[1];
                    }
                    else
                    {
                        Array.Resize(ref oResultado, oResultado.Length + 1);
                    }

                    oObjetoTabla = oTipo.InvokeMember("new", BindingFlags.CreateInstance, null, null, null);

                    oAgregar = oObjetoTabla;
                    foreach (DataColumn oColumna in oTabla.Columns)
                    {
                        if (DBNull.Value.Equals(oFila[oColumna.ColumnName]))
                        {
                            continue;
                        }
                        oPropiedad = oAgregar.GetType().GetProperty(oColumna.ColumnName);

                        switch (oFila[oColumna.ColumnName].GetType())
                        {
                            case Type _ when oFila[oColumna.ColumnName].GetType() == typeof(DateTime):
                                {
                                    oPropiedad.SetValue(oAgregar, String.Format("{0:yyyyMMdd}", oFila[oColumna.ColumnName]), default);
                                    break;
                                }
                            case Type _ when oFila[oColumna.ColumnName].GetType() == typeof(int):
                                {
                                    oPropiedad.SetValue(oAgregar, oFila[oColumna.ColumnName], default);
                                    break;
                                }
                            case Type _ when oFila[oColumna.ColumnName].GetType() == typeof(Int16) || oFila[oColumna.ColumnName].GetType() == typeof(Int32):
                                {
                                    oPropiedad.SetValue(oAgregar, oFila[oColumna.ColumnName], default);
                                    break;
                                }
                            case Type _ when oFila[oColumna.ColumnName].GetType() == typeof(decimal):
                                {
                                    oPropiedad.SetValue(oAgregar, Convert.ToDecimal(oFila[oColumna.ColumnName]), default);
                                    break;
                                }
                            case Type _ when oFila[oColumna.ColumnName].GetType() == typeof(Double):
                                {
                                    oPropiedad.SetValue(oAgregar, Convert.ToDouble(oFila[oColumna.ColumnName]), default);
                                    break;
                                }
                            default:
                                {

                                    oPropiedad.SetValue(oAgregar, string.Format("{0}", oFila[oColumna.ColumnName]), default);
                                    break;
                                }
                        }
                    }

                    oResultado[oResultado.Length - 1] = oAgregar;
                }

                oFinal = oResultado.ToList();
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                oFinal = null;
            }

            return oFinal;
        }

        //OBTENCION DE QUERY AUTOMATICO PASANDO SOLO UN DTO CON LA ESTUCTURA DE LA BD
        /// <summary>
        /// </summary>
        /// <param name="oContenedor"></param>
        /// <param name="sTabla"></param>
        /// <param name="sQuery"></param>
        /// <param name="sError"></param>
        public void ObtenerQuery(Object oContenedor, string sTabla, ref string[] sQuery, ref string sError, int oTipoBD = 1, List<string> oTablasExcluida = null)
        {
                           

            int iIndiceQuery = -1;
            string sInsertInto = "";
            string sCampos = "";
            string sValores = "";

            try
            {
                oTablasExcluidas = oTablasExcluida;
                if (sError.Trim() != "")
                    return;

                Type oMiTipoObjeto = oContenedor.GetType();
                PropertyInfo[] oPropiedades = oMiTipoObjeto.GetProperties();

                for (int iContador = 0; iContador < oPropiedades.Length; iContador++)
                {

                    string sNombPropiedad = oPropiedades[iContador].Name;
                    var oValorPropiedad = oContenedor.GetType()
                        .GetProperty(sNombPropiedad, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(oContenedor, null);

                    if (oValorPropiedad == null)
                        continue;
                    else
                    {
                        if (sTabla.Trim() == "")
                        {
                            if (ExcluirInsert(sNombPropiedad, "", ref sError))
                                continue;
                        }
                        else
                        {
                            if (ExcluirInsert(sTabla, sNombPropiedad, ref sError, oTablasExcluida))
                                continue;
                        }
                    }

                    if (oValorPropiedad.GetType() == typeof(string))
                        if (iIndiceQuery == -1)
                        {
                            if (sQuery == null)
                            {
                                iIndiceQuery = 0;
                                sQuery = new string[1];
                            }
                            else
                            {
                                iIndiceQuery = sQuery.Length;
                                Array.Resize(ref sQuery, iIndiceQuery + 1);
                            }
 
                                if (sTabla.Contains("SYS_P"))
                                    sInsertInto = string.Format("INSERT INTO [@{0}]{1}", sTabla.ToUpper(), Environment.NewLine);
                                else
                                    sInsertInto = string.Format("INSERT INTO {0}{1}", sTabla.ToUpper(), Environment.NewLine);
                                sCampos = string.Format("({0}{1}", sNombPropiedad, Environment.NewLine);
                            
                            sValores = string.Format("VALUES ('{0}'{1}", oValorPropiedad.ToString().Trim(), Environment.NewLine);

                        }
                        else
                        {

                                sCampos = string.Format("{0},{1}{2}", sCampos, sNombPropiedad, Environment.NewLine);
                            sValores = string.Format("{0},'{1}'{2}", sValores, oValorPropiedad.ToString().Replace("'", "''").Trim(), Environment.NewLine);
                        }
                    else
                    {

                        if (ExcluirInsert(sNombPropiedad, "", ref sError))
                            continue;

                        if (oValorPropiedad is IEnumerable)
                            foreach (var oListaObjeto in (IEnumerable)oContenedor.GetType().GetProperty(sNombPropiedad, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(oContenedor, null))
                            {
                                if (oListaObjeto == null)
                                    continue;
                                ObtenerQuery(oListaObjeto, sNombPropiedad, ref sQuery, ref sError, oTipoBD);
                            }
                        else
                            ObtenerQuery(oValorPropiedad, sNombPropiedad, ref sQuery, ref sError, oTipoBD);
                    }


                }

                if (iIndiceQuery != -1)
                {
                    sQuery[iIndiceQuery] = string.Format("{0} {1}) {2}) ", sInsertInto, sCampos, sValores);
                }

            }
            catch (Exception ex)
            {
                sQuery = null;
                sError = ex.Message;
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="sTabla"></param>
        /// <param name="sCampo"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public bool ExcluirInsert(string sTabla, string sCampo, ref string sError, List<string> oTablasExcluidas = null)
        {

            bool bResultado = false;

            try
            {
                if ((oTablasExcluidas != null && oTablasExcluidas.Contains(sTabla)) ||
                    sTabla.Trim() == "IntentoPorPOSDocuments" || sCampo.Trim() == "IntentoPorPOSDocuments" ||
                    sTabla.Trim() == "Tipo" || sCampo.Trim() == "Tipo" ||
                    sTabla.Trim() == "FolioApartado" || sCampo.Trim() == "FolioApartado")
                {
                    bResultado = true;
                }

            }
            catch (Exception ex)
            {
                bResultado = false;
                sError = ex.Message;
            }

            return bResultado;

        }


        #endregion

        #region Propiedades
        #endregion
    }
}
