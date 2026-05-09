internal static class DBConexion
{
    private static string GetConnectionString()
    {
        string server   = System.Configuration.ConfigurationManager
                            .AppSettings["DB_Server"];
        string database = System.Configuration.ConfigurationManager
                            .AppSettings["DB_Name"];
        string user     = System.Configuration.ConfigurationManager
                            .AppSettings["DB_User"];
        string password = System.Configuration.ConfigurationManager
                            .AppSettings["DB_Password"];

        return $"Server={server};" +
               $"Port=3306;" +
               $"Database={database};" +
               $"Uid={user};" +
               $"Pwd={password};" +
                "SslMode=Required;";
    }

    public static MySql.Data.MySqlClient.MySqlConnection ObtenerConexion()
    {
        return new MySql.Data.MySqlClient.MySqlConnection(GetConnectionString());
    }
}