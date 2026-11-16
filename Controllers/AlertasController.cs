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
    [RoutePrefix("api/alertas")]
    public class AlertasController : ApiController
    {
        private readonly IAlertaService alertaService;

        public AlertasController()
            : this(CrearServicio())
        {
        }

        public AlertasController(IAlertaService alertaService)
        {
            this.alertaService = alertaService;
        }

        // GET api/alertas/historial
        [HttpGet]
        [Route("historial")]
        public async Task<IHttpActionResult> ObtenerHistorial(
            CancellationToken cancellationToken)
        {
            var alertas = await alertaService.ObtenerHistorialAsync(
                cancellationToken);

            return Ok(new ApiResponse<List<AlertaResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = alertas.Count == 0
                    ? "No se encontraron alertas registradas."
                    : "Historial de alertas obtenido correctamente.",
                Data = alertas
            });
        }

        private static IAlertaService CrearServicio()
        {
            var connection = ConfigurationManager.ConnectionStrings[
                "SgtaConnection"];

            if (connection == null)
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ConnectionStrings:SgtaConnection.");
            }

            return new AlertaService(
                new SqlAlertaRepository(connection.ConnectionString));
        }
    }
}
