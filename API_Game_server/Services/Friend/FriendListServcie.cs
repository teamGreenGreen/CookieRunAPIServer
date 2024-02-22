using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;

namespace API_Game_Server.Services
{
    public class FriendListService : IFriendListService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendListService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<(EErrorCode, IEnumerable<FriendElement>)> FriendList(string sessionId)
        {
            // 가져온 sessionId 이용해서 클라이언트의 user_name 조회
            string sessionIdKey = string.Format("user_info:session_id:{0}",sessionId);
            string userInfo = await redisDB.GetString(sessionIdKey);
            // 가져온 string 역직렬화
            UserInfo myInfo = JsonSerializer.Deserialize<UserInfo>(userInfo);
            // 역직렬화한 string에서 user_name 프로퍼티만 가져오기
            string myName = myInfo.UserName;

            // FRIEND_RELATIONSHIP 테이블에서 친구 목록 가져오기
            string nameKey = string.Format("friend_relationship:{0}",myName);
            string[] friendUserNames = await redisDB.GetSetMembers(nameKey);
            IEnumerable<FriendElement> friendList = friendUserNames.Select(userName => new FriendElement
            {
                UserName = userName
            });
            return (EErrorCode.None, friendList);
        }
    }
}