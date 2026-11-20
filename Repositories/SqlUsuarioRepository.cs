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
    public class SqlUsuarioRepository : IUsuarioRepository
    {
        private readonly string connectionString;

        public SqlUsuarioRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("La cadena de conexión es obligatoria.", "connectionString");
            }

            this.connectionString = connectionString;
        }

        public async Task<Usuario> BuscarPorCuentaAsync(
            string cuenta,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_BuscarUsuarioXCuentaTrackIT", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("Pcuenta_n", SqlDbType.VarChar, 50).Value = cuenta;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    return new Usuario
                    {
                        UsuarioNombre = LeerTexto(reader, "Usuario"),
                        NombreCompleto = LeerTexto(reader, "NombreCompleto"),
                        Area = LeerTexto(reader, "Area"),
                        Rol = LeerTexto(reader, "Rol")
                    };
                }
            }
        }

        public async Task<List<Usuario>> BuscarPersonalTrackItPorFiltroAsync(
            string filtro,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_ListarPersonalTrackITXFiltro",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add(
                    "Pfiltro_c",
                    SqlDbType.VarChar,
                    50).Value = filtro;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    var usuarios = new List<Usuario>();

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        usuarios.Add(new Usuario
                        {
                            UsuarioNombre = LeerTexto(
                                reader,
                                "Usuario"),
                            NombreCompleto = LeerTexto(
                                reader,
                                "NombreCompleto"),
                            Area = LeerTexto(reader, "Area")
                        });
                    }

                    return usuarios;
                }
            }
        }

        public async Task<List<Usuario>> BuscarPorFiltroAsync(
            string filtro,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_sm_ListarUsuarioXFiltro",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add(
                    "Pfiltro_c",
                    SqlDbType.VarChar,
                    50).Value = filtro;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    var usuarios = new List<Usuario>();

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        usuarios.Add(new Usuario
                        {
                            UsuarioNombre = LeerTexto(
                                reader,
                                "Usuario"),
                            NombreCompleto = LeerTexto(
                                reader,
                                "NombreCompleto"),
                            Area = LeerTexto(reader, "Area")
                        });
                    }

                    return usuarios;
                }
            }
        }

        private static string LeerTexto(SqlDataReader reader, string columna)
        {
            var valor = reader[columna];
            return valor == DBNull.Value ? null : Convert.ToString(valor);
        }
    }
}
