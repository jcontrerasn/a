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
    public class SqlTareaRepository : ITareaRepository
    {
        private readonly string connectionString;

        public SqlTareaRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión es obligatoria.",
                    "connectionString");
            }

            this.connectionString = connectionString;
        }

        public async Task<TareaDetalle> ObtenerDetallePorIdAsync(
            int idTarea,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_BuscarTareaXId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("PidTarea", SqlDbType.Int).Value = idTarea;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    var tarea = new TareaDetalle
                    {
                        CodigoTarea = LeerTexto(reader, "CodigoTarea"),
                        Titulo = LeerTexto(reader, "Titulo"),
                        FechaVencimiento = reader.GetDateTime(
                            reader.GetOrdinal("FechaVencimiento")),
                        Descripcion = LeerTexto(reader, "Descripcion"),
                        UsuarioCreador = LeerTexto(reader, "UsuarioCreador"),
                        Estado = LeerTexto(reader, "Estado")
                    };

                    if (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            tarea.UsuariosAsignados.Add(new UsuarioAsignadoDetalle
                            {
                                Usuario = LeerTexto(reader, "Usuario"),
                                NombreCompleto = LeerTexto(
                                    reader, "NombreCompleto")
                            });
                        }
                    }

                    if (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            tarea.Anotaciones.Add(new AnotacionTareaDetalle
                            {
                                IdAnotacion = reader.GetInt32(
                                    reader.GetOrdinal("IdAnotacion")),
                                IdTarea = reader.GetInt32(
                                    reader.GetOrdinal("IdTarea")),
                                UsuarioAutor = LeerTexto(
                                    reader, "UsuarioAutor"),
                                Comentario = LeerTexto(
                                    reader, "Comentario"),
                                EsComentarioCierre = reader.GetBoolean(
                                    reader.GetOrdinal("EsComentarioCierre")),
                                FechaCreacion = reader.GetDateTime(
                                    reader.GetOrdinal("FechaCreacion"))
                            });
                        }
                    }

                    return tarea;
                }
            }
        }

        public async Task<List<TareaHistorial>> ObtenerHistorialAsync(
            int idTarea,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_HistorialTarea", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("PidTarea", SqlDbType.Int).Value = idTarea;

                try
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var reader = await command.ExecuteReaderAsync(
                        cancellationToken))
                    {
                        var historial = new List<TareaHistorial>();

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            historial.Add(new TareaHistorial
                            {
                                IdHistorial = reader.GetInt32(
                                    reader.GetOrdinal("IdHistorial")),
                                IdTarea = reader.GetInt32(
                                    reader.GetOrdinal("IdTarea")),
                                TipoEvento = LeerTexto(reader, "TipoEvento"),
                                Usuario = LeerTexto(reader, "Usuario"),
                                FechaEvento = reader.GetDateTime(
                                    reader.GetOrdinal("FechaEvento")),
                                Descripcion = LeerTexto(reader, "Descripcion"),
                                EstadoAnterior = LeerTexto(
                                    reader, "EstadoAnterior"),
                                EstadoNuevo = LeerTexto(reader, "EstadoNuevo"),
                                IdAnotacion = LeerEnteroNullable(
                                    reader, "IdAnotacion"),
                                CampoModificado = LeerTexto(
                                    reader, "CampoModificado"),
                                ValorAnterior = LeerTexto(
                                    reader, "ValorAnterior"),
                                ValorNuevo = LeerTexto(reader, "ValorNuevo")
                            });
                        }

                        return historial;
                    }
                }
                catch (SqlException exception)
                {
                    if (exception.Number == 50027)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        public async Task<TareaAnotacion> CrearAnotacionAsync(
            TareaAnotacion anotacion,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_ActualizarTareaAnotaciones", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("PidTarea", SqlDbType.Int).Value = anotacion.IdTarea;
                command.Parameters.Add("PusuarioAutor", SqlDbType.VarChar, 100).Value = anotacion.UsuarioAutor;
                command.Parameters.Add("Pcomentario", SqlDbType.NVarChar, -1).Value = anotacion.Comentario;
                command.Parameters.Add("PesComentarioCierre", SqlDbType.Bit).Value = anotacion.EsComentarioCierre;

                try
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var reader = await command.ExecuteReaderAsync(
                        cancellationToken))
                    {
                        if (!await reader.ReadAsync(cancellationToken))
                        {
                            return null;
                        }

                        return new TareaAnotacion
                        {
                            IdAnotacion = reader.GetInt32(
                                reader.GetOrdinal("IdAnotacion")),
                            IdTarea = reader.GetInt32(
                                reader.GetOrdinal("IdTarea")),
                            UsuarioAutor = LeerTexto(
                                reader, "UsuarioAutor"),
                            Comentario = LeerTexto(reader, "Comentario"),
                            EsComentarioCierre = reader.GetBoolean(
                                reader.GetOrdinal("EsComentarioCierre")),
                            FechaCreacion = reader.GetDateTime(
                                reader.GetOrdinal("FechaCreacion"))
                        };
                    }
                }
                catch (SqlException exception)
                {
                    if (exception.Number == 50022)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        public async Task<Tarea> CrearConInvolucradosAsync(
            Tarea tarea,
            IList<string> usuariosInvolucrados,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_InsertarNuevaTarea",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add(
                    "Ptitulo",
                    SqlDbType.NVarChar,
                    120).Value = tarea.Titulo;
                command.Parameters.Add(
                    "Pdescripcion",
                    SqlDbType.NVarChar,
                    -1).Value = tarea.Descripcion;
                command.Parameters.Add(
                    "PfechaVencimiento",
                    SqlDbType.Date).Value = tarea.FechaVencimiento.Date;
                command.Parameters.Add(
                    "PusuarioCreador",
                    SqlDbType.VarChar,
                    100).Value = tarea.UsuarioCreador;
                command.Parameters.Add(
                    "PusuariosInvolucrados",
                    SqlDbType.NVarChar,
                    -1).Value = string.Join(
                        ";",
                        usuariosInvolucrados);

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        throw new InvalidOperationException(
                            "El procedimiento no devolvió la tarea creada.");
                    }

                    var tareaCreada = new Tarea
                    {
                        IdTarea = reader.GetInt32(
                            reader.GetOrdinal("IdTarea")),
                        CodigoTarea = LeerTexto(reader, "CodigoTarea"),
                        Titulo = LeerTexto(reader, "Titulo"),
                        Descripcion = LeerTexto(reader, "Descripcion"),
                        FechaVencimiento = reader.GetDateTime(
                            reader.GetOrdinal("FechaVencimiento")),
                        UsuarioCreador = LeerTexto(
                            reader,
                            "UsuarioCreador"),
                        Estado = LeerTexto(reader, "Estado"),
                        FechaCreacion = reader.GetDateTime(
                            reader.GetOrdinal("FechaCreacion"))
                    };

                    if (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            tareaCreada.Involucrados.Add(
                                new TareaInvolucrado
                                {
                                    IdTareaInvolucrado = reader.GetInt32(
                                        reader.GetOrdinal(
                                            "IdTareaInvolucrado")),
                                    IdTarea = reader.GetInt32(
                                        reader.GetOrdinal("IdTarea")),
                                    Usuario = LeerTexto(reader, "Usuario")
                                });
                        }
                    }

                    return tareaCreada;
                }
            }
        }

        public async Task<Tarea> ActualizarAsync(
            int idTarea,
            Tarea tarea,
            IList<string> usuariosInvolucrados,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_ActualizarTarea", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("PidTarea", SqlDbType.Int).Value = idTarea;
                command.Parameters.Add("Ptitulo", SqlDbType.NVarChar, 120).Value = tarea.Titulo;
                command.Parameters.Add("Pdescripcion", SqlDbType.NVarChar, -1).Value = tarea.Descripcion;
                command.Parameters.Add("PfechaVencimiento", SqlDbType.Date).Value = tarea.FechaVencimiento.Date;
                command.Parameters.Add("PusuariosInvolucrados", SqlDbType.NVarChar, -1).Value = string.Join(";", usuariosInvolucrados);

                try
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var reader = await command.ExecuteReaderAsync(
                        cancellationToken))
                    {
                        if (!await reader.ReadAsync(cancellationToken))
                        {
                            return null;
                        }

                        var tareaActualizada = new Tarea
                        {
                            IdTarea = reader.GetInt32(
                                reader.GetOrdinal("IdTarea")),
                            CodigoTarea = LeerTexto(reader, "CodigoTarea"),
                            Titulo = LeerTexto(reader, "Titulo"),
                            Descripcion = LeerTexto(reader, "Descripcion"),
                            FechaVencimiento = reader.GetDateTime(
                                reader.GetOrdinal("FechaVencimiento")),
                            UsuarioCreador = LeerTexto(
                                reader, "UsuarioCreador"),
                            Estado = LeerTexto(reader, "Estado"),
                            FechaCreacion = reader.GetDateTime(
                                reader.GetOrdinal("FechaCreacion"))
                        };

                        if (await reader.NextResultAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                tareaActualizada.Involucrados.Add(
                                    new TareaInvolucrado
                                    {
                                        IdTareaInvolucrado = reader.GetInt32(
                                            reader.GetOrdinal("IdTareaInvolucrado")),
                                        IdTarea = reader.GetInt32(
                                            reader.GetOrdinal("IdTarea")),
                                        Usuario = LeerTexto(reader, "Usuario")
                                    });
                            }
                        }

                        return tareaActualizada;
                    }
                }
                catch (SqlException exception)
                {
                    if (exception.Number == 50017)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        public async Task<bool> EliminarAsync(
            int idTarea,
            string usuarioCreador,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_EliminarTarea", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add("PidTarea", SqlDbType.Int).Value = idTarea;
                command.Parameters.Add("PusuarioCreador", SqlDbType.VarChar, 100).Value = usuarioCreador;

                try
                {
                    await connection.OpenAsync(cancellationToken);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                    return true;
                }
                catch (SqlException exception)
                {
                    if (exception.Number == 50025)
                    {
                        return false;
                    }

                    throw;
                }
            }
        }

        public async Task<List<Tarea>> ObtenerPorFlagAsync(
            string flag,
            string usuario,
            CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "pr_trackit_ListarTareasFlag",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Parameters.Add(
                    "Pflag",
                    SqlDbType.VarChar,
                    20).Value = flag;
                command.Parameters.Add(
                    "Pusuario",
                    SqlDbType.VarChar,
                    100).Value = usuario;

                await connection.OpenAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(
                    cancellationToken))
                {
                    var tareas = new List<Tarea>();
                    var tareasPorId = new Dictionary<int, Tarea>();

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var tarea = new Tarea
                        {
                            IdTarea = reader.GetInt32(
                                reader.GetOrdinal("IdTarea")),
                            CodigoTarea = LeerTexto(reader, "CodigoTarea"),
                            Titulo = LeerTexto(reader, "Titulo"),
                            Descripcion = LeerTexto(
                                reader,
                                "Descripcion"),
                            FechaVencimiento = reader.GetDateTime(
                                reader.GetOrdinal("FechaVencimiento")),
                            UsuarioCreador = LeerTexto(
                                reader,
                                "UsuarioCreador"),
                            RolUsuarioCreador = LeerTexto(
                                reader,
                                "RolUsuarioCreador"),
                            Estado = LeerTexto(reader, "Estado"),
                            FechaCreacion = reader.GetDateTime(
                                reader.GetOrdinal("FechaCreacion"))
                        };

                        tareas.Add(tarea);
                        tareasPorId[tarea.IdTarea] = tarea;
                    }

                    if (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var idTarea = reader.GetInt32(
                                reader.GetOrdinal("IdTarea"));
                            Tarea tarea;

                            if (!tareasPorId.TryGetValue(
                                idTarea,
                                out tarea))
                            {
                                continue;
                            }

                            tarea.Involucrados.Add(
                                new TareaInvolucrado
                                {
                                    IdTareaInvolucrado = reader.GetInt32(
                                        reader.GetOrdinal(
                                            "IdTareaInvolucrado")),
                                    IdTarea = idTarea,
                                    Usuario = LeerTexto(
                                        reader,
                                        "Usuario")
                                });
                        }
                    }

                    return tareas;
                }
            }
        }

        private static string LeerTexto(SqlDataReader reader, string columna)
        {
            var valor = reader[columna];
            return valor == DBNull.Value ? null : Convert.ToString(valor);
        }

        private static int? LeerEnteroNullable(
            SqlDataReader reader,
            string columna)
        {
            var valor = reader[columna];
            return valor == DBNull.Value ? (int?)null : Convert.ToInt32(valor);
        }
    }
}
