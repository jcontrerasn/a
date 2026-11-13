using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;

namespace TrackIT_backend.Interfaces
{
    public interface ITareaService
    {
        Task<TareaDetalleResponseDto> ObtenerDetallePorIdAsync(
            int idTarea,
            CancellationToken cancellationToken);

        Task<List<HistorialTareaResponseDto>> ObtenerHistorialAsync(
            int idTarea,
            CancellationToken cancellationToken);

        Task<AnotacionTareaResponseDto> CrearAnotacionAsync(
            int idTarea,
            CrearAnotacionTareaRequestDto request,
            CancellationToken cancellationToken);

        Task<TareaResponseDto> CrearAsync(
            CrearTareaRequestDto request,
            CancellationToken cancellationToken);

        Task<TareaResponseDto> ActualizarAsync(
            int idTarea,
            ActualizarTareaRequestDto request,
            CancellationToken cancellationToken);

        Task<bool> EliminarAsync(
            int idTarea,
            string usuarioCreador,
            CancellationToken cancellationToken);

        Task<List<TareaResponseDto>> ObtenerInvolucradoEnAsync(
            string usuario,
            CancellationToken cancellationToken);

        Task<List<TareaResponseDto>> ObtenerCreadasPorUsuarioAsync(
            string usuario,
            CancellationToken cancellationToken);

        Task<List<TareaResponseDto>> ObtenerOtrasAsync(
            string usuario,
            CancellationToken cancellationToken);
    }
}
