using System;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    internal static class DBConexion
    {
        private const string ConnectionString =
            "Server=YOUR-RDS-ENDPOINT.rds.amazonaws.com;" +
            "Port=3306;" +
            "Database=WikiUnach;" +
            "Uid=WikiAdmin_01;" +
            "Pwd=Adm#1287;" +
            "SslMode=Required;";

        /// <summary>
        /// Devuelve una nueva instancia de MySqlConnection lista para abrir.
        /// Uso: using (var conn = DBConexion.ObtenerConexion()) { conn.Open(); ... }
        /// </summary>
        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
