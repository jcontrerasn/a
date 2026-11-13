using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TrackIT_backend.Dtos;
using TrackIT_backend.Interfaces;
using TrackIT_backend.Repositories;
using TrackIT_backend.Services;

namespace TrackIT_backend.Controllers
{
    [RoutePrefix("api/usuario-roles")]
    public class UsuariosRolesController : ApiController
    {
        private readonly IUsuarioRolService usuarioRolService;

        public UsuariosRolesController()
            : this(CrearServicio())
        {
        }

        public UsuariosRolesController(IUsuarioRolService usuarioRolService)
        {
            this.usuarioRolService = usuarioRolService;
        }

        // GET api/usuario-roles
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            var usuariosRoles = await usuarioRolService.ObtenerTodosAsync(
                cancellationToken);

            return Ok(new ApiResponse<List<UsuarioRolResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Relaciones usuario-rol obtenidas correctamente.",
                Data = usuariosRoles
            });
        }

        // PUT api/usuario-roles/{idUsuarioRol}
        [HttpPut]
        [Route("{idUsuarioRol:int}")]
        public async Task<IHttpActionResult> Actualizar(
            int idUsuarioRol,
            ActualizarUsuarioRolRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var usuarioRol = await usuarioRolService.ActualizarAsync(
                    idUsuarioRol,
                    request,
                    cancellationToken);

                if (usuarioRol == null)
                {
                    return Content(HttpStatusCode.NotFound,
                        new ApiResponse<object>
                        {
                            Status = (int)HttpStatusCode.NotFound,
                            Message = "No se encontró la asignación indicada.",
                            Data = null
                        });
                }

                return Ok(new ApiResponse<UsuarioRolResponseDto>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Asignación usuario-rol actualizada correctamente.",
                    Data = usuarioRol
                });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest,
                    new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = exception.Message,
                        Data = null
                    });
            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.Conflict,
                    new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Message = exception.Message,
                        Data = null
                    });
            }
        }

        // PATCH api/usuario-roles/{idUsuarioRol}/estado
        [HttpPatch]
        [Route("{idUsuarioRol:int}/estado")]
        public async Task<IHttpActionResult> CambiarEstado(
            int idUsuarioRol,
            CancellationToken cancellationToken)
        {
            try
            {
                var usuarioRol = await usuarioRolService.CambiarEstadoAsync(
                    idUsuarioRol,
                    cancellationToken);

                if (usuarioRol == null)
                {
                    return Content(HttpStatusCode.NotFound,
                        new ApiResponse<object>
                        {
                            Status = (int)HttpStatusCode.NotFound,
                            Message = "No se encontró la asignación indicada.",
                            Data = null
                        });
                }

                return Ok(new ApiResponse<UsuarioRolResponseDto>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Estado de la asignación actualizado correctamente.",
                    Data = usuarioRol
                });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest,
                    new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = exception.Message,
                        Data = null
                    });
            }
        }

        private static IUsuarioRolService CrearServicio()
        {
            var connection = ConfigurationManager.ConnectionStrings["SgtaConnection"];

            if (connection == null)
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ConnectionStrings:SgtaConnection.");
            }

            return new UsuarioRolService(
                new SqlUsuarioRolRepository(connection.ConnectionString));
        }
    }
}
