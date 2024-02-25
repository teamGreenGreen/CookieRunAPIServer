using API_Game_Server.Model.DAO;
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
            UserInfo userInfo = await redisDB.GetString<UserInfo>($"user_info:session_id:{sessionId}");
            var result = await redisDB.GetZsetRank("rank", userInfo.UserName);
            if (result == null)
            {
                // 유저가 랭킹에 존재하지 않는다. -> 아직 게임을 진행하지 않은 유저
                return EErrorCode.IsNewbie;
            }
            res.Rank = $"{result + 1}:{userInfo.MaxScore}";
            return EErrorCode.None;
        }
        public async Task<EErrorCode> LoadRanks(RanksLoadReq req, RanksLoadRes res)
        {
            SortedSetEntry[] result = await redisDB.GetZsetRanks("rank", req.Page, req.PlayerNum);
            if (result.Length == 0)
            {
                return EErrorCode.RankersNotExist;
            }
            string[] ranks = new string[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                ranks[i] = $"{result[i].Element}:{result[i].Score}";
            }
            res.Ranks = ranks;
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
