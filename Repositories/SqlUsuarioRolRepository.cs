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
    public class SqlUsuarioRolRepository : IUsuarioRolRepository
    {
        private readonly string connectionString;

        public SqlUsuarioRolRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("La cadena de conexión es obligatoria.", "connectionString");
            }

            this.connectionString = connectionString;
        }

        public async Task<List<UsuarioRol>> ObtenerTodosAsync(
            CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT
                    usuarioRol.IdUsuarioRol,
                    usuarioRol.Usuario,
                    usuarioRol.IdRol,
                    rol.Nombre AS NombreRol,
                    usuarioRol.Activo
                FROM dbo.TRACKIT_UsuarioRoles usuarioRol
                INNER JOIN dbo.TRACKIT_Roles rol
                    ON rol.IdRol = usuarioRol.IdRol
                ORDER BY usuarioRol.IdUsuarioRol";

            var usuariosRoles = new List<UsuarioRol>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        usuariosRoles.Add(new UsuarioRol
                        {
                            IdUsuarioRol = reader.GetInt32(
                                reader.GetOrdinal("IdUsuarioRol")),
                            Usuario = LeerTexto(reader, "Usuario"),
                            IdRol = reader.GetInt32(
                                reader.GetOrdinal("IdRol")),
                            NombreRol = LeerTexto(reader, "NombreRol"),
                            Activo = reader.GetBoolean(
                                reader.GetOrdinal("Activo"))
                        });
                    }
                }
            }

            return usuariosRoles;
        }

        public async Task<string> ObtenerNombreRolActivoAsync(
            string usuario,
            CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT TOP 1 r.Nombre
                FROM dbo.TRACKIT_UsuarioRoles ur
                INNER JOIN dbo.TRACKIT_Roles r ON r.IdRol = ur.IdRol
                WHERE ur.Usuario = @Usuario
                  AND ur.Activo = 1
                  AND r.Activo = 1
                ORDER BY r.IdRol";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;
                command.Parameters.Add("Usuario", SqlDbType.VarChar, 100).Value = usuario;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    return reader.IsDBNull(0) ? null : reader.GetString(0);
                }
            }
        }

        public async Task<string> ObtenerNombreRolAsync(
            int idRol,
            CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT Nombre
                FROM dbo.TRACKIT_Roles
                WHERE IdRol = @IdRol";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;
                command.Parameters.Add("IdRol", SqlDbType.Int).Value = idRol;

                await connection.OpenAsync(cancellationToken);

                var result = await command.ExecuteScalarAsync(
                    cancellationToken);

                return result == null || result == DBNull.Value
                    ? null
                    : result.ToString();
            }
        }

        public async Task<bool> ExisteAsignacionAsync(
            string usuario,
            int idRol,
            int? idUsuarioRolExcluir,
            CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.TRACKIT_UsuarioRoles
                WHERE Usuario = @Usuario
                  AND IdRol = @IdRol
                  AND (@IdUsuarioRolExcluir IS NULL
                       OR IdUsuarioRol <> @IdUsuarioRolExcluir)";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;
                command.Parameters.Add("Usuario", SqlDbType.VarChar, 100)
                    .Value = usuario;
                command.Parameters.Add("IdRol", SqlDbType.Int).Value = idRol;
                command.Parameters.Add("IdUsuarioRolExcluir", SqlDbType.Int)
                    .Value = (object)idUsuarioRolExcluir ?? DBNull.Value;

                await connection.OpenAsync(cancellationToken);

                var result = await command.ExecuteScalarAsync(
                    cancellationToken);

                return Convert.ToInt32(result) > 0;
            }
        }

        public async Task<UsuarioRol> ActualizarAsync(
            UsuarioRol usuarioRol,
            CancellationToken cancellationToken)
        {
            const string sql = @"
                UPDATE dbo.TRACKIT_UsuarioRoles
                SET Usuario = @Usuario,
                    IdRol = @IdRol
                WHERE IdUsuarioRol = @IdUsuarioRol;

                SELECT
                    usuarioRol.IdUsuarioRol,
                    usuarioRol.Usuario,
                    usuarioRol.IdRol,
                    rol.Nombre AS NombreRol,
                    usuarioRol.Activo
                FROM dbo.TRACKIT_UsuarioRoles usuarioRol
                INNER JOIN dbo.TRACKIT_Roles rol
                    ON rol.IdRol = usuarioRol.IdRol
                WHERE usuarioRol.IdUsuarioRol = @IdUsuarioRol";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;
                command.Parameters.Add("Usuario", SqlDbType.VarChar, 100)
                    .Value = usuarioRol.Usuario;
                command.Parameters.Add("IdRol", SqlDbType.Int).Value =
                    usuarioRol.IdRol;
                command.Parameters.Add("IdUsuarioRol", SqlDbType.Int).Value =
                    usuarioRol.IdUsuarioRol;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    return new UsuarioRol
                    {
                        IdUsuarioRol = reader.GetInt32(
                            reader.GetOrdinal("IdUsuarioRol")),
                        Usuario = LeerTexto(reader, "Usuario"),
                        IdRol = reader.GetInt32(
                            reader.GetOrdinal("IdRol")),
                        NombreRol = LeerTexto(reader, "NombreRol"),
                        Activo = reader.GetBoolean(
                            reader.GetOrdinal("Activo"))
                    };
                }
            }
        }

        public async Task<UsuarioRol> CambiarEstadoAsync(
            int idUsuarioRol,
            CancellationToken cancellationToken)
        {
            const string sql = @"
                UPDATE dbo.TRACKIT_UsuarioRoles
                SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END
                WHERE IdUsuarioRol = @IdUsuarioRol;

                SELECT
                    usuarioRol.IdUsuarioRol,
                    usuarioRol.Usuario,
                    usuarioRol.IdRol,
                    rol.Nombre AS NombreRol,
                    usuarioRol.Activo
                FROM dbo.TRACKIT_UsuarioRoles usuarioRol
                INNER JOIN dbo.TRACKIT_Roles rol
                    ON rol.IdRol = usuarioRol.IdRol
                WHERE usuarioRol.IdUsuarioRol = @IdUsuarioRol";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 30;
                command.Parameters.Add("IdUsuarioRol", SqlDbType.Int).Value =
                    idUsuarioRol;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    return new UsuarioRol
                    {
                        IdUsuarioRol = reader.GetInt32(
                            reader.GetOrdinal("IdUsuarioRol")),
                        Usuario = LeerTexto(reader, "Usuario"),
                        IdRol = reader.GetInt32(
                            reader.GetOrdinal("IdRol")),
                        NombreRol = LeerTexto(reader, "NombreRol"),
                        Activo = reader.GetBoolean(
                            reader.GetOrdinal("Activo"))
                    };
                }
            }
        }

        private static string LeerTexto(IDataRecord reader, string nombreColumna)
        {
            var ordinal = reader.GetOrdinal(nombreColumna);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
