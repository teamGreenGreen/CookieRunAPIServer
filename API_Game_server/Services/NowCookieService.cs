using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;
using Microsoft.VisualBasic.FileIO;

namespace API_Game_Server.Services
{
    public class NowCookieService : INowCookieService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public NowCookieService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<(EErrorCode, int)> NowCookieId(string sessionId)
        {
            // 가져온 sessionId 이용해서 클라이언트의 user_name 조회
            string sessionIdKey = string.Format("user_info:session_id:{0}", sessionId);
            string userInfo = await redisDB.GetString(sessionIdKey);
            // 가져온 string 역직렬화
            UserInfo myInfo = JsonSerializer.Deserialize<UserInfo>(userInfo);
            // 역직렬화한 string에서 diamond 프로퍼티만 가져오기
            int myDiamond = myInfo.Diamond;
            // 유저의 uid 가져오기
            long myUid = myInfo.Uid;

            string redisKey = string.Format("now_cookie_id:uid:{0}", myUid);
            string myNowCookieId = await redisDB.GetString(redisKey);

            if(myNowCookieId == "") // 현재 착용 쿠키 정보가 없는 경우 -> 1번 쿠키로 데이터 만들어서 넣음
            {
                await redisDB.SetString(redisKey, 1);
                return (EErrorCode.None, 1);
            }
            else // 현재 착용 쿠키 정보를 얻어온 경우 -> 값 return하기
            {
                return (EErrorCode.None, int.Parse(myNowCookieId));
            }
        }
    }
}