using System;

namespace TrackIT_backend.Dtos
{
    public class CrearTareaRequestDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuariosInvolucrados { get; set; }
    }
}
