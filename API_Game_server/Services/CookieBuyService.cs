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
            string filePath = "./Resources/CookieData.csv";
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited; // 필드가 구분자로 구분되어 있음을 설정
                parser.SetDelimiters(","); // 쉼표로 구분

                // CSV 파일의 각 줄을 저장할 리스트
                List<string[]> rows = new List<string[]>();

                // 파일의 모든 줄을 읽어서 리스트에 추가
                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields(); // 현재 줄의 필드 배열을 읽음
                    if (fields != null)
                    {
                        rows.Add(fields); // 리스트에 추가
                    }
                }

                return int.Parse(rows[cookieId][6]);
            }
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

            await gameDB.UpdateDiamond(myUid, myDiamond, cookieCost, myAcquiredCookieId); // DB에 정보 업데이트

            string myRedisKey = string.Format("user_info:session_id:{0}",sessionId);
            myInfo.Diamond = myDiamond - cookieCost;
            myInfo.AcquiredCookieId =  myAcquiredCookieId;
            await redisDB.SetString<UserInfo>(myRedisKey,myInfo); // redis에 정보 업데이트

            return EErrorCode.None;
        }
    }
}