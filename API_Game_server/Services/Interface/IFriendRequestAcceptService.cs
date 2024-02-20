using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendRequestAcceptService
{
    public Task<EErrorCode> FriendRequestAccept(string token, long requestId);
}
