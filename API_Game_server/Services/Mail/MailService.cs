using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using SqlKata.Execution;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;

namespace API_Game_Server.Services
{
    public class MailService : IMailService
    {
        // DB
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;

        private readonly IValidationService validationService;

        public MailService(IGameDB gameDB, IRedisDB redisDB, IValidationService validationService)
        {
            this.gameDB = gameDB;
            this.redisDB = redisDB;
            this.validationService = validationService;
        }

        public async Task<EErrorCode> GetMailListAsync(string sessionId, MailListRes res)
        {
            // sessionId를 통해 Uid 얻기
            long uid = -1;
            
            try
            {
                uid = await validationService.GetUid(sessionId);
            }
            catch
            {
                return EErrorCode.MailService_GetRedisUserInfoFail;
            }

            try
            {
                res.MailList = await gameDB.GetMailListAsync(uid);
            }
            catch
            {
                return EErrorCode.MailService_GetListFail;
            }

            return EErrorCode.None;
        }

        public Task AddMailAsync(long id, string sender, string content, int count, bool isRead, string rewardType, DateTime expiredAt)
        {
            return gameDB.AddMailAsync(id, sender, content, count, isRead, rewardType, expiredAt);
        }

        public async Task<EErrorCode> OpenMailAsync(string sessionId, MailOpenReq req)
        {
            // sessionId를 통해 Uid 얻기
            long uid = -1;

            try
            {
                uid = await validationService.GetUid(sessionId);
            }
            catch
            {
                return EErrorCode.MailService_GetRedisUserInfoFail;
            }

            // 보상하기
            MailInfo mailInfo;
            UserInfo userInfo;

            // (1) 메일, 유저 정보 가져오기
            try
            {
                mailInfo = await gameDB.GetMailAsync(uid, req.MailboxId);
                userInfo = await gameDB.GetUserByUid(uid);
            }
            catch
            {
                return EErrorCode.MailService_GetInfoFail;
            }

            // (2) 읽음 표시
            try
            {
                await gameDB.UpdateMailAsync(uid, req.MailboxId);
            }
            catch
            {
                return EErrorCode.MailService_OpenFail;
            }

            // (3) 보상하기
            try
            {
                if (mailInfo.RewardType == "none")
                {
                    return EErrorCode.None;
                }

                if (mailInfo.RewardType == "diamond")
                {
                    int newDiamond = userInfo.Diamond + mailInfo.Count;
                    await gameDB.UpdateUserInfoAsync(uid, userInfo.Level, userInfo.Exp, userInfo.Money, newDiamond, userInfo.MaxScore, userInfo.UserName);
                    userInfo.Diamond = newDiamond;
                }
                else if (mailInfo.RewardType == "money")
                {
                    int newMoney = userInfo.Money + mailInfo.Count;
                    await gameDB.UpdateUserInfoAsync(uid, userInfo.Level, userInfo.Exp, newMoney, userInfo.Diamond, userInfo.MaxScore, userInfo.UserName);
                    userInfo.Money = newMoney;
                }
            }
            catch
            {
                //TODO.김초원 : 읽었는데 보상을 실패한 경우 처리
                //(요청을 어딘가에 저장해놨다가 실패 시 다시 시도해야하는지
                //아니면 읽음 표시를 다시 없애서 보이게 해야하는지
                //아니면 for문으로 retryCount를 줘서 일정 횟수만큼 다시 시도할지?)
                return EErrorCode.MailService_RewardFail;
            }

            await redisDB.SetString<UserInfo>($"user_info:session_id:{sessionId}", userInfo);

            return EErrorCode.None;
        }
    }
}
