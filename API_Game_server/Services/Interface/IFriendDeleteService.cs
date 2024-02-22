using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendDeleteService
{
    public Task<EErrorCode> FriendDelete(string sessionId, string friendName);
}
