using System;

namespace TrackIT_backend.Entities
{
    public class TareaAnotacion
    {
        public int IdAnotacion { get; set; }
        public int IdTarea { get; set; }
        public string UsuarioAutor { get; set; }
        public string Comentario { get; set; }
        public bool EsComentarioCierre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
