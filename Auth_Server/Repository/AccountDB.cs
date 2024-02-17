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

    public async Task<int> InsertAccountAsync(string email, string saltValue, string hashingPassword)
    {
        return await queryFactory.Query("ACCOUNT").InsertAsync(new
        {
            email = email,
            salt_value = saltValue,
            password = hashingPassword
        });
    }

    public async Task<Account> GetAccount(string email, string password)
    {
        return await queryFactory.Query("ACCOUNT")
            .Where("email", email)
            .Select("email", "salt_value AS SaltValue", "password", "user_id AS UserId")
            .FirstOrDefaultAsync<Account>();
    }

    public async Task<string> GetSaltValue(Int64 userId)
    {
        return await queryFactory.Query("ACCOUNT")
            .Where("user_id", userId)
            .Select("salt_value")
            .FirstOrDefaultAsync<string>();
    }
}
