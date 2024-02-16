using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;

namespace API_Game_Server.Services
{
    public class FriendListService
    {
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;
        private readonly ValidationService validationService;
        public FriendListService(GameDB _gameDB, RedisDB _redisDB, ValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<(EErrorCode, IEnumerable<FriendElement>)> FriendList(string token)
        {
            // 토큰 유효성 검사
            string myUid = await validationService.GetUid(token);
            // 유효하지 않은 토큰이면
            if(myUid == "")
            {
                return (EErrorCode.InvalidToken, null);
            }

            // 가져온 uid 이용해서 클라이언트의 user_name 조회
            string uidKey = string.Format("user_info:uid:{0}", myUid);
            string[] arrUidValues = {"user_name"};
            string[] arrMyName = await redisDB.GetHash(uidKey, arrUidValues); // 받아올 칼럼명을 프로퍼티명으로 전달
            string myName = arrMyName[0];

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