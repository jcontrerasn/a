using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using TrackIT_backend.Entities;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Repositories
{
    public class SqlAlertaRepository : IAlertaRepository
    {
        private readonly string connectionString;

        public SqlAlertaRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión es obligatoria.",
                    "connectionString");
            }

            this.connectionString = connectionString;
        }

        public async Task<List<Alerta>> ObtenerHistorialAsync(
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_ListarHistorialAlertas",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    var alertas = new List<Alerta>();
                    var alertasPorId = new Dictionary<int, Alerta>();

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var alerta = new Alerta
                        {
                            IdAlerta = reader.GetInt32(
                                reader.GetOrdinal("IdAlerta")),
                            IdTarea = reader.GetInt32(
                                reader.GetOrdinal("IdTarea")),
                            TipoAlerta = LeerTexto(
                                reader,
                                "TipoAlerta"),
                            TituloTarea = LeerTexto(
                                reader,
                                "TituloTarea"),
                            Asunto = LeerTexto(reader, "Asunto"),
                            UsuarioCreador = LeerTexto(
                                reader,
                                "UsuarioCreador"),
                            FechaCreacionTarea = reader.GetDateTime(
                                reader.GetOrdinal(
                                    "FechaCreacionTarea")),
                            FechaVencimientoTarea = reader.GetDateTime(
                                reader.GetOrdinal(
                                    "FechaVencimientoTarea")),
                            EstadoEnvio = LeerTexto(
                                reader,
                                "EstadoEnvio"),
                            FechaCreacion = reader.GetDateTime(
                                reader.GetOrdinal("FechaCreacion")),
                            FechaEncolada = LeerFechaNullable(
                                reader,
                                "FechaEncolada"),
                            ErrorDetalle = LeerTextoNullable(
                                reader,
                                "ErrorDetalle")
                        };

                        alertas.Add(alerta);
                        alertasPorId[alerta.IdAlerta] = alerta;
                    }

                    if (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var idAlerta = reader.GetInt32(
                                reader.GetOrdinal("IdAlerta"));
                            Alerta alerta;

                            if (!alertasPorId.TryGetValue(
                                idAlerta,
                                out alerta))
                            {
                                continue;
                            }

                            alerta.Destinatarios.Add(
                                new AlertaDestinatario
                                {
                                    IdDestinatario = reader.GetInt32(
                                        reader.GetOrdinal(
                                            "IdDestinatario")),
                                    IdAlerta = idAlerta,
                                    Usuario = LeerTextoNullable(
                                        reader,
                                        "Usuario"),
                                    Correo = LeerTexto(
                                        reader,
                                        "Correo"),
                                    TipoDestinatario = LeerTexto(
                                        reader,
                                        "TipoDestinatario"),
                                    EstadoEnvio = LeerTexto(
                                        reader,
                                        "EstadoEnvio"),
                                    FechaEncolada = LeerFechaNullable(
                                        reader,
                                        "FechaEncolada"),
                                    ErrorDetalle = LeerTextoNullable(
                                        reader,
                                        "ErrorDetalle")
                                });
                        }
                    }

                    return alertas;
                }
            }
        }

        private static string LeerTexto(
            IDataRecord reader,
            string nombreColumna)
        {
            var ordinal = reader.GetOrdinal(nombreColumna);
            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetString(ordinal);
        }

        private static string LeerTextoNullable(
            IDataRecord reader,
            string nombreColumna)
        {
            var ordinal = reader.GetOrdinal(nombreColumna);
            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetString(ordinal);
        }

        private static DateTime? LeerFechaNullable(
            IDataRecord reader,
            string nombreColumna)
        {
            var ordinal = reader.GetOrdinal(nombreColumna);
            return reader.IsDBNull(ordinal)
                ? (DateTime?)null
                : reader.GetDateTime(ordinal);
        }
    }
}
