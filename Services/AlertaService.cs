using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Services
{
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository repository;

        public AlertaService(IAlertaRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<AlertaResponseDto>> ObtenerHistorialAsync(
            CancellationToken cancellationToken)
        {
            var alertas = await repository.ObtenerHistorialAsync(
                cancellationToken);

            return alertas.Select(alerta => new AlertaResponseDto
            {
                IdAlerta = alerta.IdAlerta,
                IdTarea = alerta.IdTarea,
                CodigoTarea = alerta.CodigoTarea,
                TipoAlerta = alerta.TipoAlerta,
                TituloTarea = alerta.TituloTarea,
                Asunto = alerta.Asunto,
                UsuarioCreador = alerta.UsuarioCreador,
                FechaCreacionTarea = alerta.FechaCreacionTarea,
                FechaVencimientoTarea = alerta.FechaVencimientoTarea
                    .ToString("yyyy-MM-dd"),
                EstadoEnvio = alerta.EstadoEnvio,
                FechaCreacion = alerta.FechaCreacion,
                FechaEncolada = alerta.FechaEncolada,
                ErrorDetalle = alerta.ErrorDetalle,
                Destinatarios = alerta.Destinatarios
                    .OrderBy(destinatario => destinatario.IdDestinatario)
                    .Select(destinatario =>
                        new AlertaDestinatarioResponseDto
                        {
                            IdDestinatario = destinatario.IdDestinatario,
                            IdAlerta = destinatario.IdAlerta,
                            Usuario = destinatario.Usuario,
                            Correo = destinatario.Correo,
                            TipoDestinatario =
                                destinatario.TipoDestinatario,
                            EstadoEnvio = destinatario.EstadoEnvio,
                            FechaEncolada = destinatario.FechaEncolada,
                            ErrorDetalle = destinatario.ErrorDetalle
                        })
                    .ToList()
            }).ToList();
        }
    }
}
