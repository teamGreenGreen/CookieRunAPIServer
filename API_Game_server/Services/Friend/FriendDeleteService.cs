using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;

namespace API_Game_Server.Services
{
    public class FriendDeleteService : IFriendDeleteService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendDeleteService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<EErrorCode> FriendDelete(string sessionId, string friendName)
        {
            // 가져온 sessionId 이용해서 클라이언트의 user_name 조회
            string sessionIdKey = string.Format("user_info:session_id:{0}",sessionId);
            string userInfo = await redisDB.GetString(sessionIdKey);
            // 가져온 string 역직렬화
            UserInfo myInfo = JsonSerializer.Deserialize<UserInfo>(userInfo);
            // 역직렬화한 string에서 user_name 프로퍼티만 가져오기
            string myName = myInfo.UserName;

            await gameDB.DeleteFriend(myName, friendName); // mySql 삭제
            await gameDB.DeleteFriend(friendName, myName); // mySql 삭제

            string myNameKey = string.Format("friend_relationship:{0}", myName);
            string friendNameKey = string.Format("friend_relationship:{0}", friendName);
            await redisDB.DeleteMember(myNameKey, friendName); // redis 삭제
            await redisDB.DeleteMember(friendNameKey, myName); // redis 삭제

            return EErrorCode.None;
        }
    }
}