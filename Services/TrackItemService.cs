using System.Collections.Generic;
using System.Linq;
using TrackIT_backend.Dtos;
using TrackIT_backend.Entities;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Services
{
    public class TrackItemService : ITrackItemService
    {
        private readonly ITrackItemRepository repository;

        public TrackItemService(ITrackItemRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<TrackItemDto> GetAll()
        {
            return repository.GetAll().Select(Map);
        }

        public TrackItemDto GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity == null ? null : Map(entity);
        }

        public TrackItemDto Create(CreateTrackItemDto dto)
        {
            var entity = repository.Add(new TrackItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Completed = false
            });

            return Map(entity);
        }

        private static TrackItemDto Map(TrackItem entity)
        {
            return new TrackItemDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Completed = entity.Completed,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
