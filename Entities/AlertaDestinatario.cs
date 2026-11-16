using System;

namespace TrackIT_backend.Entities
{
    public class AlertaDestinatario
    {
        public int IdDestinatario { get; set; }
        public int IdAlerta { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string TipoDestinatario { get; set; }
        public string EstadoEnvio { get; set; }
        public DateTime? FechaEncolada { get; set; }
        public string ErrorDetalle { get; set; }
    }
}
