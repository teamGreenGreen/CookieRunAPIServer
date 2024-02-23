using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendListService
{
    public Task<(EErrorCode, IEnumerable<FriendElement>)> FriendList(string sessionId);
}

