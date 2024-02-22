using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;

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
        public async Task<EErrorCode> FriendRequestDeny(long requestId)
        {
            // FRIEND_REQEUST 테이블의 해당 request_id인 row 삭제하기
            await gameDB.DeleteFriendRequestById(requestId);
            return EErrorCode.None;
        }
    }
}