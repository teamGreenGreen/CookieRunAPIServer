using API_Game_Server.Model;
using API_Game_Server.Model.DAO;
using SqlKata.Execution;

namespace API_Game_Server.Repository.Interface;

public interface IGameDB : IDisposable
{
    public Task<AttendanceInfo> GetUserAttendance(long uid);
    public Task<AttendanceInfo> SetUserAttendance(AttendanceInfo info, bool flag = true);
    public Task UpdateReward(UserInfo info, RewardItem count);
    public Task<FriendInfo> GetFriendInfo(string friendName);
    public Task<FriendRequestInfo> GetFriendRequestInfo(string fromUserName, string toUserName);
    public Task<RequestInfo> GetRequestInfo(long requestId);
    public Task<ReverseRequestInfo> GetReverseRequestInfo(string myName, string targetName);
    public Task<IEnumerable<FriendRequestElement>> GetFriendRequestList(string myName);
    public Task InsertFriendShip(string fromUserName, string toUserName);
    public Task InsertFriendRequest(string fromUserName, string toUserName);
    public Task DeleteFriendRequest(string fromUserName, string toUserName);
    public Task DeleteFriendRequestById(long requestId);
    public Task DeleteFriend(string fromUserName, string toUserName);
    public Task<IEnumerable<MailInfo>> GetMailListAsync(long id);
    public Task AddMailAsync(long id, string sender, string content, int count, bool isRead, string rewardType, DateTime expiredAt);
    public Task<MailInfo> GetMailAsync(long id, int mailboxId);
    public Task UpdateMailAsync(long id, int mailboxId);
    public Task UpdateUserInfoAsync(long uid, int newLevel, int newExp, int newMoney, int newDiamond, int newMaxScore, string userName);
    public Task<UserInfo> GetUserByUserId(Int64 userId);
    public Task<UserInfo> GetUserByUid(Int64 uid);
    public Task<UserInfo> GetUserByUserName(string userName);
    public Task<int> InsertUserGetId(Int64 userId, string userName);
    public Task InsertUserAttendance(Int64 uid);
    public Task UpdateCookieAndDiamond(long uid, int myDiamond, int cost, int newAcquiredCookieId);
}
