using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;
using Microsoft.VisualBasic.FileIO;
using API_Game_Server.Resources;

namespace API_Game_Server.Services
{
    public class CookieBuyService : ICookieBuyService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public CookieBuyService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public int ReadCookieData(int cookieId)
        {
            // DataCookie의 인스턴스를 얻어옴
            DataCookie dataCookieInstance = DataCookie.Instance;

            // 쿠키 ID에 해당하는 쿠키를 찾아서 그 쿠키의 DiamondCost를 반환
            foreach (var cookie in dataCookieInstance.Cookies)
            {
                if (cookie.ID == cookieId)
                {
                    return cookie.DiamondCost;
                }
            }

            // 쿠키를 찾지 못한 경우 -1을 반환
            return -1;
        }
        public async Task<EErrorCode> CookieBuy(string sessionId, int cookieId)
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

            int cookieCost = ReadCookieData(cookieId);

            if(myDiamond < cookieCost)
            {
                return EErrorCode.NotEnoughDiamond;
            }

            // 유저의 AcquiredCookieId 가져오기
            int myAcquiredCookieId = myInfo.AcquiredCookieId;
            int bitToSet = 1 << (cookieId - 1);
            myAcquiredCookieId |= bitToSet;

            await gameDB.UpdateCookieAndDiamond(myUid, myDiamond, cookieCost, myAcquiredCookieId); // DB에 정보 업데이트

            string myRedisKey = string.Format("user_info:session_id:{0}",sessionId);
            myInfo.Diamond = myDiamond - cookieCost;
            myInfo.AcquiredCookieId =  myAcquiredCookieId;
            await redisDB.SetString<UserInfo>(myRedisKey,myInfo); // redis에 정보 업데이트

            return EErrorCode.None;
        }
    }
}