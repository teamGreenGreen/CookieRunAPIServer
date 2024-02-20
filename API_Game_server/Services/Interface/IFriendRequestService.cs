using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendRequestService
{
    public Task<EErrorCode> FriendRequest(string token, string toUserName);
}
