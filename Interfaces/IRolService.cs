using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface IRolService
    {
        Task<List<RolResponseDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken);
    }
}
