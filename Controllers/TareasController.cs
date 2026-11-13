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
    [RoutePrefix("api/tareas")]
    public class TareasController : ApiController
    {
        private readonly ITareaService tareaService;

        public TareasController()
            : this(CrearServicio())
        {
        }

        public TareasController(ITareaService tareaService)
        {
            this.tareaService = tareaService;
        }

        // PUT api/tareas/{idTarea}
        [HttpPut]
        [Route("{idTarea:int}")]
        public async Task<IHttpActionResult> Actualizar(
            int idTarea,
            ActualizarTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tarea = await tareaService.ActualizarAsync(
                    idTarea, request, cancellationToken);

                if (tarea == null)
                {
                    return Content(HttpStatusCode.NotFound, new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "No se encontró la tarea indicada.",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<TareaResponseDto>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Tarea actualizada correctamente.",
                    Data = tarea
                });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        // DELETE api/tareas/{idTarea}?usuario={usuario}
        [HttpDelete]
        [Route("{idTarea:int}")]
        public async Task<IHttpActionResult> Eliminar(
            int idTarea,
            string usuario,
            CancellationToken cancellationToken)
        {
            try
            {
                var eliminado = await tareaService.EliminarAsync(
                    idTarea, usuario, cancellationToken);

                if (!eliminado)
                {
                    return Content(HttpStatusCode.NotFound, new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "No se encontró la tarea o el usuario no es su creador.",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Tarea eliminada correctamente.",
                    Data = null
                });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        // GET api/tareas/{idTarea}
        [HttpGet]
        [Route("{idTarea:int}")]
        public async Task<IHttpActionResult> ObtenerPorId(
            int idTarea,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "El Id de la tarea debe ser mayor que cero.",
                    Data = null
                });
            }

            var tarea = await tareaService.ObtenerDetallePorIdAsync(
                idTarea, cancellationToken);

            if (tarea == null)
            {
                return Content(HttpStatusCode.NotFound, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "No se encontró la tarea indicada.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<TareaDetalleResponseDto>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Detalle de la tarea obtenido correctamente.",
                Data = tarea
            });
        }

        // GET api/tareas/{idTarea}/historial
        [HttpGet]
        [Route("{idTarea:int}/historial")]
        public async Task<IHttpActionResult> ObtenerHistorial(
            int idTarea,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "El Id de la tarea debe ser mayor que cero.",
                    Data = null
                });
            }

            var historial = await tareaService.ObtenerHistorialAsync(
                idTarea, cancellationToken);

            if (historial == null)
            {
                return Content(HttpStatusCode.NotFound, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "No se encontró la tarea indicada.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<HistorialTareaResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Historial de la tarea obtenido correctamente.",
                Data = historial
            });
        }

        // POST api/tareas/{idTarea}/anotaciones
        [HttpPost]
        [Route("{idTarea:int}/anotaciones")]
        public async Task<IHttpActionResult> CrearAnotacion(
            int idTarea,
            CrearAnotacionTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var anotacion = await tareaService.CrearAnotacionAsync(
                    idTarea, request, cancellationToken);

                if (anotacion == null)
                {
                    return Content(HttpStatusCode.NotFound, new ApiResponse<object>
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "No se encontró la tarea indicada.",
                        Data = null
                    });
                }

                return Content(HttpStatusCode.Created,
                    new ApiResponse<AnotacionTareaResponseDto>
                    {
                        Status = (int)HttpStatusCode.Created,
                        Message = "Anotación creada correctamente.",
                        Data = anotacion
                    });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        // POST api/tareas
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Crear(
            CrearTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tarea = await tareaService.CrearAsync(
                    request,
                    cancellationToken);

                return Content(HttpStatusCode.Created,
                    new ApiResponse<TareaResponseDto>
                    {
                        Status = (int)HttpStatusCode.Created,
                        Message = "Tarea registrada correctamente.",
                        Data = tarea
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

        // GET api/tareas/involucrado/{usuario}
        [HttpGet]
        [Route("involucrado/{usuario}")]
        public async Task<IHttpActionResult> ObtenerComoInvolucrado(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "El usuario involucrado es obligatorio.",
                    Data = null
                });
            }

            var tareas = await tareaService.ObtenerInvolucradoEnAsync(
                usuario, cancellationToken);

            return Ok(new ApiResponse<List<TareaResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Tareas donde el usuario está involucrado obtenidas correctamente.",
                Data = tareas
            });
        }

        // GET api/tareas/creadas-por/{usuario}
        [HttpGet]
        [Route("creadas-por/{usuario}")]
        public async Task<IHttpActionResult> ObtenerCreadasPorUsuario(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "El usuario creador es obligatorio.",
                    Data = null
                });
            }

            var tareas = await tareaService.ObtenerCreadasPorUsuarioAsync(
                usuario, cancellationToken);

            return Ok(new ApiResponse<List<TareaResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Tareas creadas por el usuario obtenidas correctamente.",
                Data = tareas
            });
        }

        // GET api/tareas/otras/{usuario}
        [HttpGet]
        [Route("otras/{usuario}")]
        public async Task<IHttpActionResult> ObtenerOtras(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "El usuario es obligatorio.",
                    Data = null
                });
            }

            try
            {
                var tareas = await tareaService.ObtenerOtrasAsync(
                    usuario, cancellationToken);

                return Ok(new ApiResponse<List<TareaResponseDto>>
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Otras tareas obtenidas correctamente.",
                    Data = tareas
                });
            }
            catch (ArgumentException exception)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<object>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    Data = null
                });
            }
        }

        private static ITareaService CrearServicio()
        {
            var connection = ConfigurationManager.ConnectionStrings["SgtaConnection"];

            if (connection == null)
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ConnectionStrings:SgtaConnection.");
            }

            return new TareaService(
                new SqlTareaRepository(connection.ConnectionString),
                new SqlUsuarioRolRepository(connection.ConnectionString));
        }
    }
}
