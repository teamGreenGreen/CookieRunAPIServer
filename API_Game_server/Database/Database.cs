using MySqlConnector;

public class Database
{
    static string dbConnectionString;

    public static void Init(IConfiguration configuration)
    {
        dbConnectionString = configuration.GetSection("DBConnection")["MySql"]; 
    }

    public static async Task<MySqlConnection> GetMySqlConnetion()
    {
        MySqlConnection connection = new MySqlConnection(dbConnectionString);
        await connection.OpenAsync();

        return connection;
    }
}

