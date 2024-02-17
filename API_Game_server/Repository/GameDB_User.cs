using API_Game_Server.Model.DAO;
using SqlKata.Execution;
using System.Text;

namespace API_Game_Server.Repository;

public partial class GameDB : IDisposable
{
    public async Task<UserInfo> GetUserByUserId(Int64 userId)
    {
        return await queryFactory.Query("USER_INFO")
            .Select("uid", "user_id as UserId", "user_name as UserName", "level", "exp", "money", "max_score AS MaxScore", "acquired_cookie_id AS AcquiredCookieId", "diamond")
            .Where("user_id", userId)
            .FirstOrDefaultAsync<UserInfo>();
    }

    public async Task<UserInfo> GetUserByUserName(string userName)
    {
        return await queryFactory.Query("USER_INFO")
            .Select("uid", "user_id as UserId", "user_name as UserName", "level", "exp", "money", "max_score AS MaxScore", "acquired_cookie_id AS AcquiredCookieId", "diamond")
            .Where("user_name", userName)
            .FirstOrDefaultAsync<UserInfo>();
    }

    public async Task<int> InsertUserGetId(Int64 userId, string userName)
    {
        return await queryFactory.Query("USER_INFO")
            .InsertGetIdAsync<int>(new
            {
                user_id = userId,
                user_name = userName,
                level = 1,
                exp = 0,
                money = 0,
                max_score = 0,
                acquired_cookie_id = 1,
                diamond = 0,
            });
    }
}
