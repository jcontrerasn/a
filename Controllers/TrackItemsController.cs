using System.Web.Http;
using TrackIT_backend.Dtos;
using TrackIT_backend.Repositories;
using TrackIT_backend.Services;

namespace TrackIT_backend.Controllers
{
    [RoutePrefix("api/track-items")]
    public class TrackItemsController : ApiController
    {
        private readonly ITrackItemService service;

        public TrackItemsController()
            : this(new TrackItemService(new InMemoryTrackItemRepository()))
        {
        }

        public TrackItemsController(ITrackItemService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            return Ok(service.GetAll());
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetTrackItemById")]
        public IHttpActionResult GetById(int id)
        {
            var item = service.GetById(id);
            return item == null ? (IHttpActionResult)NotFound() : Ok(item);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreateTrackItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = service.Create(dto);
            return CreatedAtRoute("GetTrackItemById", new { id = item.Id }, item);
        }
    }
}
