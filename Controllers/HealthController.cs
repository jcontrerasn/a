using System;
using System.Web.Http;

namespace TrackIT_backend.Controllers
{
    public class HealthController : ApiController
    {
        // GET api/health
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                message = "API funcionando correctamente",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
