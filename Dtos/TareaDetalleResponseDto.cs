using System;
using System.Collections.Generic;

namespace TrackIT_backend.Dtos
{
    public class TareaDetalleResponseDto
    {
        public string CodigoTarea { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioCreador { get; set; }
        public string Estado { get; set; }
        public List<UsuarioAsignadoResponseDto> UsuariosAsignados { get; set; }
        public List<AnotacionTareaResponseDto> Anotaciones { get; set; }

        public TareaDetalleResponseDto()
        {
            UsuariosAsignados = new List<UsuarioAsignadoResponseDto>();
            Anotaciones = new List<AnotacionTareaResponseDto>();
        }
    }
}
