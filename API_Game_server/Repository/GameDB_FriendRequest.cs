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
    public async Task<FriendInfo> GetFriendInfo(string friendName)
    {
        return await queryFactory.Query("USER_INFO")
        .Select("user_name as UserName")
        .Where("user_name", friendName)
        .FirstOrDefaultAsync<FriendInfo>();
    }
    public async Task<FriendRequestInfo> GetFriendRequestInfo(string fromUserName, string toUserName)
    {
        return await queryFactory.Query("FRIEND_REQUEST")
        .Select("from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("from_user_name", fromUserName)
        .Where("to_user_name", toUserName)
        .FirstOrDefaultAsync<FriendRequestInfo>();
    }
    public async Task<RequestInfo> GetRequestInfo(long requestId)
    {
        return await queryFactory.Query("FRIEND_REQUEST")
        .Select("request_id as RequestId", "from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("request_id", requestId)
        .FirstOrDefaultAsync<RequestInfo>();
    }
    public async Task<ReverseRequestInfo> GetReverseRequestInfo(string myName, string targetName)
    {
        return await queryFactory.Query("FRIEND_REQUEST")
        .Select("from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("from_user_name", targetName)
        .Where("to_user_name", myName)
        .FirstOrDefaultAsync<ReverseRequestInfo>();
    }
    public async Task<IEnumerable<FriendRequestElement>> GetFriendRequestList(string myName)
    {
        return await queryFactory.Query("FRIEND_REQUEST")
        .Select("request_id as RequestId", "from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("to_user_name", myName)
        .GetAsync<FriendRequestElement>();
    }
    public async Task InsertFriendShip(string fromUserName, string toUserName)
    {
        await queryFactory.Query("FRIEND_RELATIONSHIP").InsertAsync(new { from_user_name = fromUserName, to_user_name = toUserName});
    }
    public async Task InsertFriendRequest(string fromUserName, string toUserName)
    {
        await queryFactory.Query("FRIEND_REQUEST").InsertAsync(new { from_user_name = fromUserName, to_user_name = toUserName });
    }
    public async Task DeleteFriendRequest(string fromUserName, string toUserName)
    {
        await queryFactory.Query("FRIEND_REQUEST")
        .Where("from_user_name",fromUserName)
        .Where("to_user_name",toUserName)
        .DeleteAsync();
    }
    public async Task DeleteFriendRequestById(long requestId)
    {
        await queryFactory.Query("FRIEND_REQUEST")
        .Where("request_id", requestId)
        .DeleteAsync();
    }
    public async Task DeleteFriend(string fromUserName, string toUserName)
    {
        await queryFactory.Query("FRIEND_RELATIONSHIP")
        .Where("from_user_name",fromUserName)
        .Where("to_user_name",toUserName)
        .DeleteAsync();
    }
}