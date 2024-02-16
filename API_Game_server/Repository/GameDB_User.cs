using API_Game_Server.Model.DAO;
using SqlKata.Execution;
using System.Text;

namespace API_Game_Server.Repository;

public partial class GameDB : IDisposable
{
    public async Task<UserInfo> GetUserByUid(Int64 uid)
    {
        return await queryFactory.Query("USER_INFO")
            .Select("info_id as InfoId", "uid", "user_name as UserName", "level", "exp", "money", "max_score AS MaxScore", "acquired_cookie_id AS AcquiredCookieId", "diamond")
            .Where("uid", uid)
            .FirstOrDefaultAsync<UserInfo>();
    }

    public async Task<UserInfo> GetUserByUserName(string userName)
    {
        return await queryFactory.Query("USER_INFO")
            .Select("info_id as InfoId", "uid", "user_name as UserName", "level", "exp", "money", "max_score AS MaxScore", "acquired_cookie_id AS AcquiredCookieId", "diamond")
            .Where("user_name", userName)
            .FirstOrDefaultAsync<UserInfo>();
    }

    public async Task<int> InsertUserGetId(Int64 uid, string userName)
    {
        return await queryFactory.Query("USER_INFO")
            .InsertGetIdAsync<int>(new
            {
                uid = uid,
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
