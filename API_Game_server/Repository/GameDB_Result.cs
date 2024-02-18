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
    public Task<ResultUserInfo> GetUserInfoAsync(long id)
    {
        return queryFactory.Query("USER_INFO")
        .Select("uid", "level", "exp", "money", "diamond", "max_score as MaxScore", "user_name as UserName")
        .Where("uid", id)
        .FirstOrDefaultAsync<ResultUserInfo>();
    }

    public Task UpdateUserInfoAsync(long uid, int newLevel, int newExp, int newMoney, int newDiamond, int newMaxScore, string userName)
    {
        return queryFactory.Query("USER_INFO").Where("uid", uid).UpdateAsync(new
        {
            level = newLevel,
            exp = newExp,
            money = newMoney,
            diamond = newDiamond,
            max_score = newMaxScore,
            user_name = userName
        });
    }
}