using System.Collections.Generic;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface ITrackItemService
    {
        IEnumerable<TrackItemDto> GetAll();

        TrackItemDto GetById(int id);

        TrackItemDto Create(CreateTrackItemDto dto);
    }
}
