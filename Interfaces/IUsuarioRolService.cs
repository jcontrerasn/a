using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface IUsuarioRolService
    {
        Task<List<UsuarioRolResponseDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken);

        Task<UsuarioRolResponseDto> ActualizarAsync(
            int idUsuarioRol,
            ActualizarUsuarioRolRequestDto request,
            CancellationToken cancellationToken);

        Task<UsuarioRolResponseDto> CambiarEstadoAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken);

        Task<UsuarioRolResponseDto> CrearAsync(
            CrearUsuarioRolRequestDto request,
            CancellationToken cancellationToken);

        Task<bool> EliminarAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken);
    }
}
