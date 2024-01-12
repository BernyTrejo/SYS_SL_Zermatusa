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

namespace SYS_PR_Acceso.CS.Controladores
{

    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "SYS_CO_Acceso")]
    public class SYS_CO_Acceso : ControllerBase
    {
        private readonly SYS_DTO_Conexion oConexion;
        private readonly IConfiguration _config;

        //se  obtienen parametros de conexion y configuracion
        public SYS_CO_Acceso(IOptions<SYS_DTO_Conexion> oConexionContexto, IConfiguration config)

        {
            oConexion = oConexionContexto.Value;
            _config = config;
        }

        /// <summary>
        /// Funcion encargada de generar login
        /// </summary>
        /// <param name="oLogin"></param>
        /// <returns>Regresa el token de autenticacion</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SYS_DTO_Login))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(SYS_DTO_Login oLogin)
        {
              
            SYS_CL_ZERMAT_CN oCapaNegocio = null;
            SecurityToken sToken = null;
            string sError = "";
            string sJsonCuerpo = "";
            int iCode = 500;
            try
            {
                //se valida instancia objeto conexion
                if (oConexion == null)
                {
                    oLogin.Descripcion = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, oLogin.Descripcion);
                }

                //se instancia capa de negocios
                oCapaNegocio = new SYS_CL_ZERMAT_CN(oConexion);

                //se valida el objeto capa de negocios se instanciara bien
                if (oCapaNegocio == null)
                {
                    oLogin.Descripcion = "No se logro obtener la conexion a la BD";
                    return StatusCode(StatusCodes.Status500InternalServerError, oLogin.Descripcion);
                }

                //se genera string para token
                sToken = oCapaNegocio.ObtenerTokenUsuario(ref oLogin, _config.GetSection("AppSettings:Token").Value, _config.GetSection("AppSettings:DiasVigencia").Value, ref sError, ref iCode);

                if (sToken == null && sError.Trim() == "")
                    sError = "Ocurrio un error desconocido al obtener el token";
    
                // se valida si hay error
                if (sError.Trim() != "")
                {                  
                    return StatusCode(StatusCodes.Status404NotFound, String.Format("Ocurrio un error al obtener el token: {0}. Error: {1}", oLogin.User, sError));
                }

                // se obtiene objeto token
                var tokenHandler = new JwtSecurityTokenHandler();

                //se genera token con todos los parametros obtenidos
                oLogin.Token = tokenHandler.WriteToken(sToken);

                return StatusCode(StatusCodes.Status200OK, oLogin);
            }
            catch (Exception ex)
            {
                //en caso de fallar se manda estado de error
                ModelState.AddModelError("ErroresAccesoLogin", String.Format("Ocurrio un error al obtener el token Error: {0}", ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }

    }
}
