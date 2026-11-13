using System;
using System.Collections.Generic;

namespace TrackIT_backend.Entities
{
    public class Tarea
    {
        public int IdTarea { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string UsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
        public string RolUsuarioCreador { get; set; }
        public IList<TareaInvolucrado> Involucrados { get; set; }

        public Tarea()
        {
            Involucrados = new List<TareaInvolucrado>();
        }
    }
}
