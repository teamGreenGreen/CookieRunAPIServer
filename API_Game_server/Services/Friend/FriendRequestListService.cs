using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;

namespace API_Game_Server.Services
{
    public class FriendRequestListService : IFriendRequestListService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendRequestListService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<(EErrorCode, IEnumerable<FriendRequestElement>)> FriendRequestList(string token)
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
            string[] arrUidValues = {"UserName"};
            string[] arrMyName = await redisDB.GetHash(uidKey, arrUidValues); // 받아올 칼럼명을 프로퍼티명으로 전달
            string myName = arrMyName[0];

            // FRIEND_REQUEST 테이블에서 to_user_name가 나의 user_name인 항목들 모두 SELECT 해서 담기
            return (EErrorCode.None, await gameDB.GetFriendRequestList(myName));
        }
    }
}