using API_Game_Server;
using Auth_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace Auth_Server.Repository;

public class AccountDB : IDisposable
{
    private readonly IOptions<DBConfig> dbConfig;
    private readonly IDbConnection dbConnection;
    private readonly QueryFactory queryFactory;

    public AccountDB(IOptions<DBConfig> dbConfig)
    {
        this.dbConfig = dbConfig;

        dbConnection = new MySqlConnection(dbConfig.Value.AccountDB);
        dbConnection.Open();

        queryFactory = new QueryFactory(dbConnection, new SqlKata.Compilers.MySqlCompiler());

    }

    public void Dispose()
    {
        dbConnection.Close();
    }

    public async Task<int> InsertAccountAsync(string userName, string saltValue, string hashingPassword)
    {
        object account = new
        {
            user_name = userName,
            salt_value = saltValue,
            password = hashingPassword
        };

        return await queryFactory.Query("ACCOUNT").InsertAsync(account);
    }

    public async Task<Account> GetAccount(string userName, string password)
    {
        return await queryFactory.Query("ACCOUNT")
            .Where("user_name", userName)
            .Select("user_name AS UserName", "password", "uid")
            .FirstOrDefaultAsync<Account>();
    }
}
