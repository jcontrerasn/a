using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TrackIT_backend.Entities;

namespace TrackIT_backend.Interfaces
{
    public interface IUsuarioRolRepository
    {
        Task<List<UsuarioRol>> ObtenerTodosAsync(
            CancellationToken cancellationToken);

        Task<string> ObtenerNombreRolActivoAsync(
            string usuario,
            CancellationToken cancellationToken);

        Task<string> ObtenerNombreRolAsync(
            int idRol,
            CancellationToken cancellationToken);

        Task<bool> ExisteAsignacionAsync(
            string usuario,
            int idRol,
            int? idUsuarioRolExcluir,
            CancellationToken cancellationToken);

        Task<UsuarioRol> ActualizarAsync(
            UsuarioRol usuarioRol,
            CancellationToken cancellationToken);

        Task<UsuarioRol> CambiarEstadoAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken);

        Task<UsuarioRol> CrearAsync(
            UsuarioRol usuarioRol,
            CancellationToken cancellationToken);

        Task<bool> EliminarAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken);
    }
}
