using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using StackExchange.Redis;

namespace API_Game_Server.Services.Interface;

public interface IRankService
{
    public Task<EErrorCode> GetRank(string sessionId, RankGetRes res);
    public Task<EErrorCode> LoadRanks(RanksLoadReq req, RanksLoadRes res);
    public Task<EErrorCode> GetSizeOfRanks(RankSizeRes res);
}
