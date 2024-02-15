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

    public async Task<EErrorCode> CreateAccountAsync(string userName, string password)
    {
        // 해시 함수 적용 예정
        string saltValue = Security.GenerateSaltString();
        string hashingPassword = Security.GenerateHashingPassword(saltValue, password);

        object account = new
        {
            user_name = userName,
            salt_value = saltValue,
            password = hashingPassword
        };

        int count = await queryFactory.Query("ACCOUNT").InsertAsync(account);

        return count == 1 ? EErrorCode.None : EErrorCode.CreateAccountFail;
    }

    public async Task<(EErrorCode, Account?)> VerifyUser(string userName, string password)
    {
        Account userAccount = await queryFactory.Query("ACCOUNT")
            .Where("user_name", userName)
            .Select("user_name AS UserName", "password", "uid")
            .FirstOrDefaultAsync<Account>();

        if (userAccount is null)
        {
            return (EErrorCode.LoginFailUserNotExist, null);
        }

        return (EErrorCode.None, userAccount);
    }
}