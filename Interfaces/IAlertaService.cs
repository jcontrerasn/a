using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface IAlertaService
    {
        Task<List<AlertaResponseDto>> ObtenerHistorialAsync(
            CancellationToken cancellationToken);
    }
}
