using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;
using TrackIT_backend.Entities;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Services
{
    public class TareaService : ITareaService
    {
        private readonly ITareaRepository repository;
        private readonly IUsuarioRolRepository usuarioRolRepository;

        public TareaService(
            ITareaRepository repository,
            IUsuarioRolRepository usuarioRolRepository)
        {
            this.repository = repository;
            this.usuarioRolRepository = usuarioRolRepository;
        }

        public async Task<TareaDetalleResponseDto> ObtenerDetallePorIdAsync(
            int idTarea,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentException(
                    "El Id de la tarea debe ser mayor que cero.",
                    "idTarea");
            }

            var tarea = await repository.ObtenerDetallePorIdAsync(
                idTarea, cancellationToken);

            if (tarea == null)
            {
                return null;
            }

            return new TareaDetalleResponseDto
            {
                Titulo = tarea.Titulo,
                FechaVencimiento = tarea.FechaVencimiento,
                Descripcion = tarea.Descripcion,
                UsuarioCreador = tarea.UsuarioCreador,
                Estado = tarea.Estado,
                UsuariosAsignados = tarea.UsuariosAsignados
                    .Select(usuario => new UsuarioAsignadoResponseDto
                    {
                        Usuario = usuario.Usuario,
                        NombreCompleto = usuario.NombreCompleto
                    })
                    .ToList(),
                Anotaciones = tarea.Anotaciones
                    .Select(anotacion => new AnotacionTareaResponseDto
                    {
                        IdAnotacion = anotacion.IdAnotacion,
                        IdTarea = anotacion.IdTarea,
                        UsuarioAutor = anotacion.UsuarioAutor,
                        Comentario = anotacion.Comentario,
                        EsComentarioCierre = anotacion.EsComentarioCierre,
                        FechaCreacion = anotacion.FechaCreacion
                    })
                    .ToList()
            };
        }

        public async Task<List<HistorialTareaResponseDto>> ObtenerHistorialAsync(
            int idTarea,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentException(
                    "El Id de la tarea debe ser mayor que cero.",
                    "idTarea");
            }

            var historial = await repository.ObtenerHistorialAsync(
                idTarea, cancellationToken);

            if (historial == null)
            {
                return null;
            }

            return historial.Select(evento => new HistorialTareaResponseDto
            {
                IdHistorial = evento.IdHistorial,
                IdTarea = evento.IdTarea,
                TipoEvento = evento.TipoEvento,
                Usuario = evento.Usuario,
                FechaEvento = evento.FechaEvento,
                Descripcion = evento.Descripcion,
                EstadoAnterior = evento.EstadoAnterior,
                EstadoNuevo = evento.EstadoNuevo,
                IdAnotacion = evento.IdAnotacion,
                CampoModificado = evento.CampoModificado,
                ValorAnterior = evento.ValorAnterior,
                ValorNuevo = evento.ValorNuevo
            }).ToList();
        }

        public async Task<AnotacionTareaResponseDto> CrearAnotacionAsync(
            int idTarea,
            CrearAnotacionTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentException(
                    "El Id de la tarea debe ser mayor que cero.",
                    "idTarea");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.UsuarioAutor))
            {
                throw new ArgumentException("El usuario autor es obligatorio.", "request");
            }

            if (string.IsNullOrWhiteSpace(request.Comentario))
            {
                throw new ArgumentException("El comentario es obligatorio.", "request");
            }

            var anotacion = await repository.CrearAnotacionAsync(
                new TareaAnotacion
                {
                    IdTarea = idTarea,
                    UsuarioAutor = request.UsuarioAutor.Trim(),
                    Comentario = request.Comentario.Trim(),
                    EsComentarioCierre = request.EsComentarioCierre
                },
                cancellationToken);

            if (anotacion == null)
            {
                return null;
            }

            return new AnotacionTareaResponseDto
            {
                IdAnotacion = anotacion.IdAnotacion,
                IdTarea = anotacion.IdTarea,
                UsuarioAutor = anotacion.UsuarioAutor,
                Comentario = anotacion.Comentario,
                EsComentarioCierre = anotacion.EsComentarioCierre,
                FechaCreacion = anotacion.FechaCreacion
            };
        }

        public async Task<TareaResponseDto> CrearAsync(
            CrearTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Los datos de la tarea son obligatorios.",
                    "request");
            }

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                throw new ArgumentException(
                    "El título es obligatorio.",
                    "request");
            }

            if (request.Titulo.Trim().Length > 120)
            {
                throw new ArgumentException(
                    "El título no puede superar los 120 caracteres.",
                    "request");
            }

            if (string.IsNullOrWhiteSpace(request.Descripcion))
            {
                throw new ArgumentException(
                    "La descripción es obligatoria.",
                    "request");
            }

            if (request.FechaVencimiento == default(DateTime))
            {
                throw new ArgumentException(
                    "La fecha de vencimiento es obligatoria.",
                    "request");
            }

            if (string.IsNullOrWhiteSpace(request.UsuarioCreador))
            {
                throw new ArgumentException(
                    "El usuario creador es obligatorio.",
                    "request");
            }

            if (request.UsuarioCreador.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "El usuario creador no puede superar los 100 caracteres.",
                    "request");
            }

            var usuariosInvolucrados = (request.UsuariosInvolucrados
                ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(usuario => usuario.Trim())
                .Where(usuario => !string.IsNullOrWhiteSpace(usuario))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (usuariosInvolucrados.Count == 0)
            {
                throw new ArgumentException(
                    "Debe indicar al menos un usuario involucrado.",
                    "request");
            }

            if (usuariosInvolucrados.Any(usuario => usuario.Length > 100))
            {
                throw new ArgumentException(
                    "El usuario involucrado no puede superar los 100 caracteres.",
                    "request");
            }

            var tareaCreada = await repository.CrearConInvolucradosAsync(
                new Tarea
                {
                    Titulo = request.Titulo.Trim(),
                    Descripcion = request.Descripcion.Trim(),
                    FechaVencimiento = request.FechaVencimiento.Date,
                    UsuarioCreador = request.UsuarioCreador.Trim()
                },
                usuariosInvolucrados,
                cancellationToken);

            return Map(tareaCreada);
        }

        public async Task<TareaResponseDto> ActualizarAsync(
            int idTarea,
            ActualizarTareaRequestDto request,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentException(
                    "El Id de la tarea debe ser mayor que cero.",
                    "idTarea");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Titulo))
            {
                throw new ArgumentException("El título es obligatorio.", "request");
            }

            if (string.IsNullOrWhiteSpace(request.Descripcion))
            {
                throw new ArgumentException("La descripción es obligatoria.", "request");
            }

            if (request.FechaVencimiento == default(DateTime))
            {
                throw new ArgumentException(
                    "La fecha de vencimiento es obligatoria.",
                    "request");
            }

            var usuariosInvolucrados = (request.UsuariosInvolucrados ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(usuario => usuario.Trim())
                .Where(usuario => !string.IsNullOrWhiteSpace(usuario))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (usuariosInvolucrados.Count == 0)
            {
                throw new ArgumentException(
                    "Debe indicar al menos un usuario involucrado.",
                    "request");
            }

            if (usuariosInvolucrados.Any(usuario => usuario.Length > 100))
            {
                throw new ArgumentException(
                    "El usuario involucrado no puede superar los 100 caracteres.",
                    "request");
            }

            var tareaActualizada = await repository.ActualizarAsync(
                idTarea,
                new Tarea
                {
                    IdTarea = idTarea,
                    Titulo = request.Titulo.Trim(),
                    Descripcion = request.Descripcion.Trim(),
                    FechaVencimiento = request.FechaVencimiento
                },
                usuariosInvolucrados,
                cancellationToken);

            return tareaActualizada == null ? null : Map(tareaActualizada);
        }

        public async Task<bool> EliminarAsync(
            int idTarea,
            string usuarioCreador,
            CancellationToken cancellationToken)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentException(
                    "El Id de la tarea debe ser mayor que cero.",
                    "idTarea");
            }

            if (string.IsNullOrWhiteSpace(usuarioCreador))
            {
                throw new ArgumentException(
                    "El usuario creador es obligatorio.",
                    "usuarioCreador");
            }

            return await repository.EliminarAsync(
                idTarea,
                usuarioCreador.Trim(),
                cancellationToken);
        }

        public async Task<List<TareaResponseDto>> ObtenerInvolucradoEnAsync(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new ArgumentException(
                    "El usuario involucrado es obligatorio.",
                    "usuario");
            }

            var tareas = await repository.ObtenerInvolucradoEnAsync(
                usuario.Trim(), cancellationToken);

            return tareas.Select(Map).ToList();
        }

        public async Task<List<TareaResponseDto>> ObtenerOtrasAsync(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new ArgumentException(
                    "El usuario es obligatorio.",
                    "usuario");
            }

            var usuarioNormalizado = usuario.Trim();
            var rolUsuario = await usuarioRolRepository
                .ObtenerNombreRolActivoAsync(
                    usuarioNormalizado, cancellationToken);

            if (string.IsNullOrWhiteSpace(rolUsuario))
            {
                throw new ArgumentException(
                    "El usuario no está registrado o no tiene un rol activo en el sistema.",
                    "usuario");
            }

            var tareas = await repository.ObtenerOtrasAsync(
                usuarioNormalizado, rolUsuario, cancellationToken);

            return tareas.Select(Map).ToList();
        }

        public async Task<List<TareaResponseDto>> ObtenerCreadasPorUsuarioAsync(
            string usuario,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new ArgumentException(
                    "El usuario creador es obligatorio.",
                    "usuario");
            }

            var tareas = await repository.ObtenerCreadasPorUsuarioAsync(
                usuario.Trim(), cancellationToken);

            return tareas.Select(Map).ToList();
        }

        private static TareaResponseDto Map(Tarea tarea)
        {
            return new TareaResponseDto
            {
                IdTarea = tarea.IdTarea,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaVencimiento = tarea.FechaVencimiento.ToString("yyyy-MM-dd"),
                UsuarioCreador = tarea.UsuarioCreador,
                RolUsuarioCreador = tarea.RolUsuarioCreador,
                Estado = tarea.Estado,
                FechaCreacion = tarea.FechaCreacion,
                UsuariosInvolucrados = tarea.Involucrados
                    .OrderBy(involucrado => involucrado.IdTareaInvolucrado)
                    .Select(involucrado => involucrado.Usuario)
                    .ToList()
            };
        }
    }
}
