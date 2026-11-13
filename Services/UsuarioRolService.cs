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
    public class UsuarioRolService : IUsuarioRolService
    {
        private readonly IUsuarioRolRepository repository;

        public UsuarioRolService(IUsuarioRolRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<UsuarioRolResponseDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken)
        {
            var usuariosRoles = await repository.ObtenerTodosAsync(
                cancellationToken);

            return usuariosRoles.Select(usuarioRol => new UsuarioRolResponseDto
            {
                IdUsuarioRol = usuarioRol.IdUsuarioRol,
                Usuario = usuarioRol.Usuario,
                NombreRol = usuarioRol.NombreRol,
                Activo = usuarioRol.Activo
            }).ToList();
        }

        public async Task<UsuarioRolResponseDto> ActualizarAsync(
            int idUsuarioRol,
            ActualizarUsuarioRolRequestDto request,
            CancellationToken cancellationToken)
        {
            if (idUsuarioRol <= 0)
            {
                throw new ArgumentException(
                    "El Id de la asignación debe ser mayor que cero.");
            }

            if (request == null)
            {
                throw new ArgumentException(
                    "Los datos de la asignación son obligatorios.");
            }

            if (string.IsNullOrWhiteSpace(request.Usuario))
            {
                throw new ArgumentException(
                    "El usuario es obligatorio.");
            }

            if (request.IdRol <= 0)
            {
                throw new ArgumentException(
                    "El Id del rol debe ser mayor que cero.");
            }

            var usuario = request.Usuario.Trim();
            var nombreRol = await repository.ObtenerNombreRolAsync(
                request.IdRol,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                throw new ArgumentException(
                    "No existe el rol indicado.");
            }

            if (await repository.ExisteAsignacionAsync(
                usuario,
                request.IdRol,
                idUsuarioRol,
                cancellationToken))
            {
                throw new InvalidOperationException(
                    "Ya existe una asignación para ese usuario y rol.");
            }

            var usuarioRol = await repository.ActualizarAsync(
                new UsuarioRol
                {
                    IdUsuarioRol = idUsuarioRol,
                    Usuario = usuario,
                    IdRol = request.IdRol,
                    NombreRol = nombreRol
                },
                cancellationToken);

            return usuarioRol == null
                ? null
                : new UsuarioRolResponseDto
                {
                    IdUsuarioRol = usuarioRol.IdUsuarioRol,
                    Usuario = usuarioRol.Usuario,
                    NombreRol = usuarioRol.NombreRol,
                    Activo = usuarioRol.Activo
                };
        }

        public async Task<UsuarioRolResponseDto> CambiarEstadoAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken)
        {
            if (idUsuarioRol <= 0)
            {
                throw new ArgumentException(
                    "El Id de la asignación debe ser mayor que cero.");
            }

            var usuarioRol = await repository.CambiarEstadoAsync(
                idUsuarioRol,
                cancellationToken);

            return usuarioRol == null
                ? null
                : new UsuarioRolResponseDto
                {
                    IdUsuarioRol = usuarioRol.IdUsuarioRol,
                    Usuario = usuarioRol.Usuario,
                    NombreRol = usuarioRol.NombreRol,
                    Activo = usuarioRol.Activo
                };
        }
    }
}
