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
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private readonly IRolService rolService;

        public RolesController()
            : this(CrearServicio())
        {
        }

        public RolesController(IRolService rolService)
        {
            this.rolService = rolService;
        }

        // GET api/roles
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            var roles = await rolService.ObtenerTodosAsync(
                cancellationToken);

            return Ok(new ApiResponse<List<RolResponseDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = "Roles obtenidos correctamente.",
                Data = roles
            });
        }

        private static IRolService CrearServicio()
        {
            var connection = ConfigurationManager.ConnectionStrings[
                "SgtaConnection"];

            if (connection == null)
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ConnectionStrings:SgtaConnection.");
            }

            return new RolService(
                new SqlRolRepository(connection.ConnectionString));
        }
    }
}
