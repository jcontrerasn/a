using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioCuentaResponse> BuscarPorCuentaAsync(
            string cuenta,
            CancellationToken cancellationToken);

        Task<List<PersonalTrackItResponseDto>>
            BuscarPersonalTrackItPorFiltroAsync(
                string filtro,
                CancellationToken cancellationToken);

        Task<List<UsuarioCuentaResponse>> BuscarPorFiltroAsync(
            string filtro,
            CancellationToken cancellationToken);
    }
}
