using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Entities;

namespace TrackIT_backend.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> BuscarPorCuentaAsync(
            string cuenta,
            CancellationToken cancellationToken);

        Task<List<Usuario>> BuscarPersonalTrackItPorFiltroAsync(
            string filtro,
            CancellationToken cancellationToken);
    }
}
