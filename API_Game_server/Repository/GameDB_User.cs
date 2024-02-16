using API_Game_Server.Model.DAO;
using SqlKata.Execution;

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
}
