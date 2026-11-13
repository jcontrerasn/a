using System.ComponentModel.DataAnnotations;

namespace TrackIT_backend.Dtos
{
    public class CrearAnotacionTareaRequestDto
    {
        [Required(ErrorMessage = "El usuario autor es obligatorio.")]
        [StringLength(100)]
        public string UsuarioAutor { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        public string Comentario { get; set; }

        public bool EsComentarioCierre { get; set; }
    }
}
