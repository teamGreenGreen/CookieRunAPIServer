using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IFriendRequestListService
{
    public Task<(EErrorCode, IEnumerable<FriendRequestElement>)> FriendRequestList(string token);
}
