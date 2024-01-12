using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SYS_PR_Nucleo.Clases.DTO;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SYS_PR_Zermat.CN.Clases;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace SYS_PR_Zermat.CS.Controladores
{
    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "SYS_CO_Movimientos")]
    public class SYS_CO_Movimientos : Controller
    {
        private readonly SYS_DTO_Conexion oConexion;
        private readonly IConfiguration _config;
        private string sUsuario = "";
        private string sPass = "";

        public SYS_CO_Movimientos(IOptions<SYS_DTO_Conexion> oConexionContexto, IConfiguration config)

        {
            oConexion = oConexionContexto.Value;
            _config = config;
        }

        /// <summary>
        /// </summary>
        /// <param name="oMovimiento"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Venta")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SYS_DTO_Venta))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Venta(SYS_DTO_Venta oMovimiento)
        {

            SYS_CL_ZERMAT_CN oCapaNegocio = null;
            SecurityToken sToken = null;
            string sError = "";
            string sJsonCuerpo = "";
            string sHeader = "";
            string sDatosUsuario = "";
            int iCode = 500;
            try
            {

                if (oConexion == null)
                {
                    sError = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, sError);
                }

                oCapaNegocio = new SYS_CL_ZERMAT_CN(oConexion);

                if (oCapaNegocio == null)
                {
                    sError = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, sError);
                }


                 if(!oCapaNegocio.GenerarMovimiento(ref oMovimiento, "V", ref sError, ref iCode)) 
                {
                    sError = "Ocurrio un error desconocido al generar la venta";
                }

                if (sError.Trim() != "")
                {

                    return StatusCode(StatusCodes.Status404NotFound, String.Format("Ocurrio un error al generar la venta : {0}. Error: {1}", oMovimiento.SYS_FOLIO, sError));
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("ErroresMovimientos", String.Format("Ocurrio un error al generar venta Error: {0}", ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="oArticulos"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ObtenerArticulo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SYS_DTO_Articulos))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerArticulo(SYS_DTO_Articulos oArticulos)
        {

            SYS_CL_ZERMAT_CN oCapaNegocio = null;
            string sError = "";
            int iCode = 500;
            try
            {

                //se valida objeto conexion
                if (oConexion == null)
                {
                    sError = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, sError);
                }

                //se intancia capa de negocios
                oCapaNegocio = new SYS_CL_ZERMAT_CN(oConexion);

                if (oCapaNegocio == null)
                {
                    sError = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, sError);
                }

                //se llama capa de negocios para obtener articulo
                if (!oCapaNegocio.ObtieneArticulos(ref oArticulos, ref sError, ref iCode))
                {
                    sError = "Ocurrio un error desconocido al obtener los articulos";
                }

                //en caso de error
                if (sError.Trim() != "")
                {

                    return StatusCode(StatusCodes.Status404NotFound, String.Format("Ocurrio un error al obtener el articulo. Error: {0}", sError));
                }

                //respuesta correcta se regresa el articulo
                return StatusCode(StatusCodes.Status200OK, oArticulos);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("ErroresMovimientos", String.Format("Ocurrio un error al obtener el articulo Error: {0}", ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }
    }
}
