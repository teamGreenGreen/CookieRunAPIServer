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
    public async Task<TestUserInfo> GetUserInfo(int id)
    {
        return await queryFactory.Query("USER_INFO") 
        .Select("Id", "Level", "Exp", "Money", "Max_Score")
        .Where("Id", id)
        .FirstOrDefaultAsync<TestUserInfo>();
    }

    public async void ChangeDB(int id, object obj)
    {
        // SQLKata 쿼리 생성
        Query query = new Query("USER_INFO").Where("Id", id).AsUpdate(obj);
        await queryFactory.ExecuteAsync(query);
    }
}