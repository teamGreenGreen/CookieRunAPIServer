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
    public async Task<FriendShipInfo> GetFriendShipInfo(string myName, string friendName)
    {
        return await queryFactory.Query("FRIEND_RELATIONSHIP")
        .Select("from_user_name as FromUserName","to_user_name as ToUserName")
        .Where("from_user_name", myName)
        .Where("to_user_name", friendName)
        .FirstOrDefaultAsync<FriendShipInfo>();
    }
    public async Task<FriendRequestInfo> GetFriendRequestInfo(string myName, string friendName)
    {
        return await queryFactory.Query("FRIEND_REQUEST")
        .Select("from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("from_user_name", myName)
        .Where("to_user_name", friendName)
        .FirstOrDefaultAsync<FriendRequestInfo>();
    }
    public async Task<FriendCountInfo> GetMyFriendCountInfo(string myName)
    {
        return await queryFactory.Query("USER_INFO")
        .Select("friend_count as FriendCount")
        .Where("user_name",myName)
        .FirstOrDefaultAsync<FriendCountInfo>();
    }

    public async Task<ReverseFriendShipInfo> GetReverseFriendShipInfo(string myName, string friendName)
    {
        return await queryFactory.Query("FRIEND_REQUSET")
        .Select("from_user_name as FromUserName", "to_user_name as ToUserName")
        .Where("from_user_name", friendName)
        .Where("to_user_name", myName)
        .FirstOrDefaultAsync<ReverseFriendShipInfo>();
    }

    public async Task InsertFriendRequest(string myName, string friendName)
    {
        var friendshipData = new []
        {
            new {from_user_name = myName, to_user_name = friendName},
            new {from_user_name = friendName, to_user_name = myName}
        };

        await queryFactory.Query("FRIEND_REQUEST").InsertAsync(friendshipData);
    }
}