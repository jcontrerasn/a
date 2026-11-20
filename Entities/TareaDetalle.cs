using System;
using System.Collections.Generic;

namespace TrackIT_backend.Entities
{
    public class TareaDetalle
    {
        public string CodigoTarea { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioCreador { get; set; }
        public string Estado { get; set; }
        public IList<UsuarioAsignadoDetalle> UsuariosAsignados { get; set; }
        public IList<AnotacionTareaDetalle> Anotaciones { get; set; }

        public TareaDetalle()
        {
            UsuariosAsignados = new List<UsuarioAsignadoDetalle>();
            Anotaciones = new List<AnotacionTareaDetalle>();
        }
    }
}
