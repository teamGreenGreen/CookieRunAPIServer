using Dapper;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace API_Game_Server.Repository;

public class AccountDB : IDisposable
{
    // 설정 값을 관리하고 주입하기 위한 인터페이스
    private readonly IOptions<DBConfig> dbConfig; 
    private readonly IDbConnection dbConnection;
    private readonly QueryFactory queryFactory;

    public AccountDB(IOptions<DBConfig> _dbConfig)
    {
        dbConfig = _dbConfig;

        dbConnection = new MySqlConnection(dbConfig.Value.AccountDb);
        dbConnection.Open();

        queryFactory = new QueryFactory(dbConnection, new SqlKata.Compilers.MySqlCompiler());
    }

    // IDisposable.Dispose는 DI 컨테이너에 의해 관리되는 클래스의 리소스를 정리할 때 사용하는 함수
    public void Dispose()
    {
        dbConnection.Close();
    }

    public async Task<UserInfo> GetUserInfo(int id)
    {
        return await queryFactory.Query("Account")
            .Select("LoginId", "HashedPassword", "CreatedAt")
            .Where("Id", id)
            .FirstOrDefaultAsync<UserInfo>();
    }
}