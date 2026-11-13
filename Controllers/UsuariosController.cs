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
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        private readonly IUsuarioService usuarioService;

        public UsuariosController()
            : this(CrearServicio())
        {
        }

        public UsuariosController(IUsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        // GET api/usuarios/cuenta/{cuenta}
        [HttpGet]
        [Route("cuenta/{cuenta}")]
        public async Task<IHttpActionResult> BuscarPorCuenta(
            string cuenta,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cuenta))
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "La cuenta es obligatoria.",
                    Data = null
                });
            }

            var usuario = await usuarioService.BuscarPorCuentaAsync(
                cuenta, cancellationToken);

            if (usuario == null)
            {
                return Content(HttpStatusCode.NotFound, new ApiResponse<UsuarioCuentaResponse>
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Este usuario no está registrado o no tiene un rol activo.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<UsuarioCuentaResponse>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Usuario obtenido correctamente.",
                Data = usuario
            });
        }

        // POST api/usuarios/personal-trackit/buscar-por-filtro
        [HttpPost]
        [Route("personal-trackit/buscar-por-filtro")]
        public async Task<IHttpActionResult> BuscarPersonalTrackItPorFiltro(
            BuscarUsuariosPorFiltroRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var usuarios = await usuarioService
                    .BuscarPersonalTrackItPorFiltroAsync(
                        request == null ? null : request.Pfiltro_c,
                        cancellationToken);

                return Ok(new ApiResponse<List<PersonalTrackItResponseDto>>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = usuarios.Count == 0
                        ? "No se encontró personal de TrackIT para el filtro indicado."
                        : "Personal de TrackIT obtenido correctamente.",
                    Data = usuarios
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

        private static IUsuarioService CrearServicio()
        {
            var connection = ConfigurationManager.ConnectionStrings["SgtaConnection"];

            if (connection == null)
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ConnectionStrings:SgtaConnection.");
            }

            return new UsuarioService(
                new SqlUsuarioRepository(connection.ConnectionString),
                new SqlUsuarioRolRepository(connection.ConnectionString));
        }
    }
}
