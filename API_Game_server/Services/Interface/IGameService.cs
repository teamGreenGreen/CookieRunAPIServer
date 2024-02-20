using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IGameService
{
    public Task<(EErrorCode, int)> CreateUserGameData(Int64 userId, string userName);
    public Task<EErrorCode> CreateUserMailBox(Int64 uid);
}
