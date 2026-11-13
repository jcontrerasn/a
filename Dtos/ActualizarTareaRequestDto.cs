using System;
using System.ComponentModel.DataAnnotations;

namespace TrackIT_backend.Dtos
{
    public class ActualizarTareaRequestDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(120)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe indicar al menos un usuario involucrado.")]
        public string UsuariosInvolucrados { get; set; }
    }
}
