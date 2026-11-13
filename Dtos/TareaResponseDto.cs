using System;
using System.Collections.Generic;

namespace TrackIT_backend.Dtos
{
    public class TareaResponseDto
    {
        public int IdTarea { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string FechaVencimiento { get; set; }
        public string UsuarioCreador { get; set; }
        public string RolUsuarioCreador { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<string> UsuariosInvolucrados { get; set; }

        public TareaResponseDto()
        {
            UsuariosInvolucrados = new List<string>();
        }
    }
}
