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

    public async Task<EErrorCode> CreateAccountAsync(string accountName, string password)
    {
        // 해시 함수 적용 필요
        string saltValue = "1234";
        string hashingPassword = password;

        object account = new
        {
            account_name = accountName,
            salt_value = saltValue,
            password = hashingPassword
        };

        int count = await queryFactory.Query("ACCOUNT").InsertAsync(account);

        return count == 1 ? EErrorCode.None : EErrorCode.CreateAccountFail;
    }

    public async Task<(EErrorCode, Int64)> VerifyUser(string accountName, string password)
    {
        Account userAccount = await queryFactory.Query("ACCOUNT")
            .Where("account_name", accountName)
            .FirstOrDefaultAsync<Account>();

        if (userAccount is null)
        {
            return (EErrorCode.LoginFailUserNotExist, 0);
        }

        return (EErrorCode.None, userAccount.Uid);
    }
}
