using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IUsuarioRolRepository usuarioRolRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IUsuarioRolRepository usuarioRolRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.usuarioRolRepository = usuarioRolRepository;
        }

        public async Task<UsuarioCuentaResponse> BuscarPorCuentaAsync(
            string cuenta,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cuenta))
            {
                throw new ArgumentException("La cuenta es obligatoria.", "cuenta");
            }

            var cuentaValidada = cuenta.Trim();
            var rol = await usuarioRolRepository.ObtenerNombreRolActivoAsync(
                cuentaValidada, cancellationToken);

            if (string.IsNullOrWhiteSpace(rol))
            {
                return null;
            }

            var usuario = await usuarioRepository.BuscarPorCuentaAsync(
                cuentaValidada, cancellationToken);

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.UsuarioNombre))
            {
                return null;
            }

            return new UsuarioCuentaResponse
            {
                Usuario = usuario.UsuarioNombre,
                NombreCompleto = usuario.NombreCompleto,
                Area = usuario.Area,
                Rol = rol
            };
        }

        public async Task<List<PersonalTrackItResponseDto>>
            BuscarPersonalTrackItPorFiltroAsync(
                string filtro,
                CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                throw new ArgumentException(
                    "El filtro es obligatorio.");
            }

            var usuarios = await usuarioRepository
                .BuscarPersonalTrackItPorFiltroAsync(
                    filtro.Trim(),
                    cancellationToken);

            return usuarios.Select(usuario => new PersonalTrackItResponseDto
            {
                Usuario = usuario.UsuarioNombre,
                NombreCompleto = usuario.NombreCompleto,
                Area = usuario.Area
            }).ToList();
        }
    }
}
