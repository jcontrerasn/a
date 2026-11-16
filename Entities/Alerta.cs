using System;
using System.Collections.Generic;

namespace TrackIT_backend.Entities
{
    public class Alerta
    {
        public int IdAlerta { get; set; }
        public int IdTarea { get; set; }
        public string TipoAlerta { get; set; }
        public string TituloTarea { get; set; }
        public string Asunto { get; set; }
        public string UsuarioCreador { get; set; }
        public DateTime FechaCreacionTarea { get; set; }
        public DateTime FechaVencimientoTarea { get; set; }
        public string EstadoEnvio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEncolada { get; set; }
        public string ErrorDetalle { get; set; }
        public IList<AlertaDestinatario> Destinatarios { get; set; }

        public Alerta()
        {
            Destinatarios = new List<AlertaDestinatario>();
        }
    }
}
