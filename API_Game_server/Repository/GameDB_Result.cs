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
    public async Task<ResultUserInfo> GetUserInfo(long id)
    {
        return await queryFactory.Query("USER_INFO") 
        .Select("uid", "level", "exp", "money", "max_score as MaxScore", "user_name as UserName")
        .Where("uid", id)
        .FirstOrDefaultAsync<ResultUserInfo>();
    }

    public async void ChangeDB(long id, int newLevel, int newExp, int newMoneyPoint, int newMaxScore)
    {
        // SQLKata 쿼리 생성
        try
        {
            await queryFactory.Query("USER_INFO").Where("uid", id).UpdateAsync(new
            {
                level = newLevel,
                exp = newExp,
                money = newMoneyPoint,
                max_score = newMaxScore
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }
}