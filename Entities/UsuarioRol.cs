namespace TrackIT_backend.Entities
{
    public class UsuarioRol
    {
        public int IdUsuarioRol { get; set; }
        public string Usuario { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public bool Activo { get; set; }
    }
}
