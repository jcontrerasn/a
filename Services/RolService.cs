using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Dtos;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository repository;

        public RolService(IRolRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<RolResponseDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken)
        {
            var roles = await repository.ObtenerTodosAsync(
                cancellationToken);

            return roles.Select(rol => new RolResponseDto
            {
                IdRol = rol.IdRol,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Activo = rol.Activo
            }).ToList();
        }
    }
}
