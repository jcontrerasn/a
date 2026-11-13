using System;

namespace TrackIT_backend.Entities
{
    public class TareaHistorial
    {
        public int IdHistorial { get; set; }
        public int IdTarea { get; set; }
        public string TipoEvento { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaEvento { get; set; }
        public string Descripcion { get; set; }
        public string EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; }
        public int? IdAnotacion { get; set; }
        public string CampoModificado { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
    }
}
