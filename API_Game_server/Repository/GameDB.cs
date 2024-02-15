using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using System.Data;

namespace API_Game_Server.Repository;

public partial class GameDB : IDisposable
{
    // 설정 값을 관리하고 주입하기 위한 인터페이스
    private readonly IOptions<DBConfig> dbConfig;
    private readonly IDbConnection dbConnection;
    private readonly QueryFactory queryFactory;

    public GameDB(IOptions<DBConfig> _dbConfig)
    {
        dbConfig = _dbConfig;

        dbConnection = new MySqlConnection(dbConfig.Value.GameDB); // 해당 부분을 변경해야 함
        dbConnection.Open();

        queryFactory = new QueryFactory(dbConnection, new SqlKata.Compilers.MySqlCompiler());
    }

    // IDisposable.Dispose는 DI 컨테이너에 의해 관리되는 클래스의 리소스를 정리할 때 사용하는 함수
    public void Dispose()
    {
        dbConnection.Close();
    }
}