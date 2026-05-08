using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    /// <summary>
    /// Sistema de notificaciones in-app.
    ///
    /// Requiere ejecutar en la BD:
    ///   CREATE TABLE tbl_WikiUnach_Notificaciones (
    ///       ID_Notificacion VARCHAR(50) PRIMARY KEY,
    ///       ID_Usuario      VARCHAR(50) NOT NULL,
    ///       Tipo            VARCHAR(30) NOT NULL,
    ///       Mensaje         TEXT        NOT NULL,
    ///       ID_Pagina       VARCHAR(50) NULL,
    ///       Leida           TINYINT(1)  NOT NULL DEFAULT 0,
    ///       Fecha_Creacion  DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ///       INDEX idx_usuario_leida (ID_Usuario, Leida),
    ///       FOREIGN KEY (ID_Usuario) REFERENCES tbl_WikiUnach_Usuarios(ID_Usuario)
    ///           ON DELETE CASCADE
    ///   );
    /// </summary>
    public static class Notificaciones
    {
        public class Item
        {
            public string IdNotificacion { get; set; }
            public string Tipo { get; set; }
            public string Mensaje { get; set; }
            public string IdPagina { get; set; }
            public bool Leida { get; set; }
            public DateTime Fecha { get; set; }
        }

        // ── CREAR ─────────────────────────────────────────────────────────────────
        public static void Crear(string idUsuarioDestino, string tipo,
                                  string mensaje, string idPagina = null)
        {
            if (string.IsNullOrEmpty(idUsuarioDestino)) return;
            // No notificarse a uno mismo
            if (idUsuarioDestino == SesionUsuario.ID_Usuario) return;

            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO tbl_WikiUnach_Notificaciones
                            (ID_Notificacion, ID_Usuario, Tipo, Mensaje, ID_Pagina)
                        VALUES
                            (@id, @user, @tipo, @msg, @pag)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@user", idUsuarioDestino);
                        cmd.Parameters.AddWithValue("@tipo", tipo);
                        cmd.Parameters.AddWithValue("@msg", mensaje);
                        if (string.IsNullOrEmpty(idPagina))
                            cmd.Parameters.Add("@pag", MySqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.AddWithValue("@pag", idPagina);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { /* notificación no crítica - no romper flujo */ }
        }

        // ── OBTENER ──────────────────────────────────────────────────────────────
        public static List<Item> Obtener(string idUsuario, int max = 30)
        {
            var lista = new List<Item>();
            if (string.IsNullOrEmpty(idUsuario)) return lista;

            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"
                        SELECT ID_Notificacion, Tipo, Mensaje, ID_Pagina, Leida, Fecha_Creacion
                        FROM   tbl_WikiUnach_Notificaciones
                        WHERE  ID_Usuario = @id
                        ORDER  BY Fecha_Creacion DESC
                        LIMIT  @max";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.Parameters.AddWithValue("@max", max);
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                                lista.Add(new Item
                                {
                                    IdNotificacion = r["ID_Notificacion"].ToString(),
                                    Tipo = r["Tipo"].ToString(),
                                    Mensaje = r["Mensaje"].ToString(),
                                    IdPagina = r["ID_Pagina"] == DBNull.Value
                                                     ? null : r["ID_Pagina"].ToString(),
                                    Leida = Convert.ToInt32(r["Leida"]) == 1,
                                    Fecha = Convert.ToDateTime(r["Fecha_Creacion"])
                                });
                    }
                }
            }
            catch { /* devolver lista vacía si falla */ }

            return lista;
        }

        // ── CONTAR NO LEÍDAS ─────────────────────────────────────────────────────
        public static int ContarNoLeidas(string idUsuario)
        {
            if (string.IsNullOrEmpty(idUsuario)) return 0;
            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM tbl_WikiUnach_Notificaciones " +
                        "WHERE ID_Usuario = @id AND Leida = 0", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch { return 0; }
        }

        // ── MARCAR COMO LEÍDA ────────────────────────────────────────────────────
        public static void MarcarComoLeida(string idNotificacion)
        {
            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE tbl_WikiUnach_Notificaciones SET Leida = 1 " +
                        "WHERE ID_Notificacion = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idNotificacion);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public static void MarcarTodasComoLeidas(string idUsuario)
        {
            if (string.IsNullOrEmpty(idUsuario)) return;
            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE tbl_WikiUnach_Notificaciones SET Leida = 1 " +
                        "WHERE ID_Usuario = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}
