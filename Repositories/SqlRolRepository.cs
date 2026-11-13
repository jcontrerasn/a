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
    public class SqlRolRepository : IRolRepository
    {
        private readonly string connectionString;

        public SqlRolRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión es obligatoria.",
                    "connectionString");
            }

            this.connectionString = connectionString;
        }

        public async Task<List<Rol>> ObtenerTodosAsync(
            CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT IdRol, Nombre, Descripcion, Activo
                FROM dbo.TRACKIT_Roles
                ORDER BY IdRol";

            var roles = new List<Rol>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        roles.Add(new Rol
                        {
                            IdRol = reader.GetInt32(
                                reader.GetOrdinal("IdRol")),
                            Nombre = LeerTexto(reader, "Nombre"),
                            Descripcion = LeerTexto(reader, "Descripcion"),
                            Activo = reader.GetBoolean(
                                reader.GetOrdinal("Activo"))
                        });
                    }
                }
            }

            return roles;
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
    }
}
