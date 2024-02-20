using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendRequestDenyService
{
    public Task<EErrorCode> FriendRequestDeny(string token, long requestId);
}
