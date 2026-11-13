using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Entities;

namespace TrackIT_backend.Interfaces
{
    public interface ITareaRepository
    {
        Task<TareaDetalle> ObtenerDetallePorIdAsync(
            int idTarea,
            CancellationToken cancellationToken);

        Task<List<TareaHistorial>> ObtenerHistorialAsync(
            int idTarea,
            CancellationToken cancellationToken);

        Task<TareaAnotacion> CrearAnotacionAsync(
            TareaAnotacion anotacion,
            CancellationToken cancellationToken);

        Task<Tarea> CrearConInvolucradosAsync(
            Tarea tarea,
            IList<string> usuariosInvolucrados,
            CancellationToken cancellationToken);

        Task<Tarea> ActualizarAsync(
            int idTarea,
            Tarea tarea,
            IList<string> usuariosInvolucrados,
            CancellationToken cancellationToken);

        Task<bool> EliminarAsync(
            int idTarea,
            string usuarioCreador,
            CancellationToken cancellationToken);

        Task<List<Tarea>> ObtenerInvolucradoEnAsync(
            string usuario,
            CancellationToken cancellationToken);

        Task<List<Tarea>> ObtenerCreadasPorUsuarioAsync(
            string usuario,
            CancellationToken cancellationToken);

        Task<List<Tarea>> ObtenerOtrasAsync(
            string usuario,
            string rolUsuario,
            CancellationToken cancellationToken);
    }
}
