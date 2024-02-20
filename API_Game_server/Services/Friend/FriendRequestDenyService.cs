using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;

namespace API_Game_Server.Services
{
    public class FriendRequestDenyService : IFriendRequestDenyService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendRequestDenyService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<EErrorCode> FriendRequestDeny(string token, long requestId)
        {
            // 토큰 유효성 검사
            string myUid = await validationService.GetUid(token);
            // 유효하지 않은 토큰이면
            if(myUid == "")
            {
                return EErrorCode.InvalidToken;
            }

            // FRIEND_REQEUST 테이블의 해당 request_id인 row 삭제하기
            await gameDB.DeleteFriendRequestById(requestId);
            return EErrorCode.None;
        }
    }
}