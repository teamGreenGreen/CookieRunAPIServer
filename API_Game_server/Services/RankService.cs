using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;
using StackExchange.Redis;

namespace API_Game_Server.Services
{
    public class RankService : IRankService
    {
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public RankService(IRedisDB redisDB, IValidationService validationService)
        {
            this.redisDB = redisDB;
            this.validationService = validationService;
        }
        public async Task<EErrorCode> GetRank(string sessionId, RankGetRes res)
        {
            long userUid = await validationService.GetUid(sessionId);
            var result = await redisDB.GetZsetRank("rank", userUid.ToString());
            if (result == null)
            {
                // 유저가 랭킹에 존재하지 않는다. -> 아직 게임을 진행하지 않은 유저
                return EErrorCode.IsNewbie;
            }
            res.Rank = (long)result + 1;
            return EErrorCode.None;
        }
        public async Task<EErrorCode> LoadRanks(RanksLoadReq req, RanksLoadRes res)
        {
            RedisValue[] result = await redisDB.GetZsetRanks("rank", req.Page);
            if (result.Length == 0)
            {
                return EErrorCode.RankersNotExist;
            }
            res.Ranks = Array.ConvertAll(result, x => (string)x);
            return EErrorCode.None;
        }
        public async Task<EErrorCode> GetSizeOfRanks(RankSizeRes res)
        {
            long result = await redisDB.GetZsetSize("rank");
            if (result == 0)
            {
                // 랭크가 없다.
                return EErrorCode.NoBodyInRanking;
            }
            res.Size = result;
            return EErrorCode.None;
        }
    }
}
