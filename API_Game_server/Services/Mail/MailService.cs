using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;

namespace API_Game_Server.Services
{
    public class MailService
    {
        // DB
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;

        private readonly ValidationService validationService;

        private IEnumerable<MailInfo> mailInfo;

        public MailService(GameDB _gameDB, RedisDB _redisDB, ValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }

        public async Task<EErrorCode> GetMailList(MailListReq req)
        {
            // 토큰 확인
            string stringUid = await validationService.GetUid(req.Token);
            if (stringUid == "")
            {
                return EErrorCode.InvalidToken;
            }

            long uid = long.Parse(stringUid);

            mailInfo = await gameDB.GetMailList(uid);

            return EErrorCode.None;
        }

        public async Task<EErrorCode> AddMailList(long _id, string _sender, string _content, int _count, bool _is_read, string _rewardType, DateTime _expiredAt)
        {
            return await gameDB.AddMailList(_id, _sender, _content, _count, _is_read, _rewardType, _expiredAt);
        }

        public async Task<EErrorCode> DeleteMailList(MailListReq req)
        {
            string stringUid = await validationService.GetUid(req.Token);
            if (stringUid == "")
            {
                return EErrorCode.InvalidToken;
            }

            long uid = long.Parse(stringUid);

            EErrorCode errorCode = EErrorCode.None;
            errorCode = await gameDB.DeleteMailAndGetReward(uid, req.MailboxId);

            if(errorCode != EErrorCode.None)
            {
                return errorCode;
            }

            return EErrorCode.None;
        }
    }
}
