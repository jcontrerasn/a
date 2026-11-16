using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Entities;

namespace TrackIT_backend.Interfaces
{
    public interface IAlertaRepository
    {
        Task<List<Alerta>> ObtenerHistorialAsync(
            CancellationToken cancellationToken);
    }
}
