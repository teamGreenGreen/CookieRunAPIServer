using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic.FileIO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace API_Game_Server.Services
{
    public class GameResultService : IGameResultService
    {
        // GameData
        private readonly Dictionary<int/*ItemID*/, ItemData> itemData = new Dictionary<int, ItemData>();
        private readonly Dictionary<int/*Level*/, UserLevelData> userLevelData = new Dictionary<int, UserLevelData>();
        private readonly Dictionary<int/*CookieID*/, CookieData> cookieData = new Dictionary<int, CookieData>();

        // DB
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;

        // Service
        private readonly IValidationService validationService;
        private readonly IMailService mailService;

        // 
        private readonly int maxMoney = 99999;
        private UserInfo userInfo;
        private int totalScore = 0;
        private int totalMoney = 0;

        public GameResultService(IGameDB gameDB, IRedisDB redisDB, IValidationService validationService, IMailService mailService)
        {
            this.gameDB = gameDB;
            this.redisDB = redisDB;
            this.validationService = validationService;
            this.mailService = mailService;
            ReadGameData();
        }

        public async Task<EErrorCode> ValidateRequestAsync(string sessionId, GameResultReq req)
        {
            try
            {
                // 레디스에서 데이터 가져오기
                string sessionIdKey = string.Format("user_info:session_id:{0}", sessionId);
                string redisFindResult = await redisDB.GetString(sessionIdKey);

                // 가져온 json 문자열을 userInfo로 역직렬화
                userInfo = JsonSerializer.Deserialize<UserInfo>(redisFindResult);
            }
            catch
            {
                // TODO.김초원 : 로그아웃 처리

                return EErrorCode.GameResultService_GetRedisUserInfoFail;
            }

            // 플레이 검증
            return ValidatePlay(req);
        }

        public async Task<EErrorCode> GiveRewardsAsync(string sessionId, GameResultReq req, GameResultRes res)
        {
            int newMoney = 0;
            int newExp = 0;
            int newLevel = 0;
            int newMaxScore = 0;

            try
            {
                newMoney = CalcMoney();
                Tuple<int, int> tupleValue = CalcExpAndLevel();
                newExp = tupleValue.Item1;
                newLevel = tupleValue.Item2;
                newMaxScore = CalcMaxScore();
            }
            catch
            {
                //TODO.김초원 : 게임은 했는데 보상 계산에서 에러난 경우
                return EErrorCode.GameResultService_RewardCalcFail;
            }

            try
            {
                await UpdateUserInfoAsync(userInfo.Uid, newLevel, newExp, newMoney, userInfo.Diamond, newMaxScore, userInfo.UserName);
            }
            catch
            {
                //TODO.김초원 : 유저 정보가 업데이트 되지 않은 경우
                return EErrorCode.GameResultService_DBUserInfoUpdateFail;
            }

            try
            {
                bool bLevelUp = newLevel != userInfo.Level;
                if (bLevelUp)
                    AddReward(newLevel);
            }
            catch
            {
                //TODO.김초원 : 레벨 업은 했는데 보상이 안 나간 경우
                // Log를 남겨 놓고 실제로 레벨 업을 했는데 보상이 안 나간 경우 로그를 확인해서 보상을 해야하나?
                return EErrorCode.GameResultService_AddLevelUpRewardFail;
            }

            UserInfo redisUserInfo = new UserInfo
            {
                Uid = userInfo.Uid,
                UserName = userInfo.UserName,
                Level = newLevel,
                Exp = newExp,
                Money = newMoney,
                MaxScore = newMaxScore,
                Diamond = userInfo.Diamond
            };

            // Redis 저장
            try
            {
                await redisDB.SetZset("rank", userInfo.UserName, newMaxScore);
                await redisDB.SetString<UserInfo>($"user_info:session_id:{sessionId}", redisUserInfo);
            }
            catch
            {
                return EErrorCode.GameResultService_RedisUpdateError;
            }

            res.Exp = newExp;
            res.Level = newLevel;
            res.Money = newMoney;
            if (res.Level + 1 > userLevelData.Count)
            {
                res.MaxExp = 99999999;
            }
            else
            {
                res.MaxExp = userLevelData[res.Level + 1].MinExp;
            }

            return EErrorCode.None;
        }

        public int CalcMaxScore()
        {
            int newMaxScore = userInfo.MaxScore;
            if (newMaxScore < totalScore)
            {
                newMaxScore = totalScore;
            }

            return newMaxScore;
        }

        public void AddReward(int newLevel)
        {
            int rewardCount = 0;
            for (int i = userInfo.Level + 1; i <= newLevel; i++)
            {
                rewardCount += i * 10;
                DateTime sevenDaysLater = DateTime.Now.AddDays(7);
                _ = mailService.AddMailAsync(userInfo.Uid, "운영자", "레벨" + i + "달성을 축하합니다.", rewardCount, false, "diamond", sevenDaysLater);
            }
        }

        public int CalcMoney()
        {
            return userInfo.Money + totalMoney < maxMoney ? userInfo.Money + totalMoney : maxMoney;
        }

        public Tuple<int, int> CalcExpAndLevel()
        {
            int newExp = userInfo.Exp + totalScore;
            int newLevel = userInfo.Level;
            for (int level = userInfo.Level + 1; level <= userLevelData.Count; level++)
            {
                if (newExp < userLevelData[level].MinExp)
                {
                    break;
                }

                newLevel = level;
                newExp -= userLevelData[newLevel].MinExp;
            }

            return Tuple.Create(newExp, newLevel);
        }

        public Task UpdateUserInfoAsync(long uid, int newLevel, int newExp, int newMoney, int newDiamond, int newMaxScore, string userName)
        {
            if (newMoney != userInfo.Money || newExp != userInfo.Exp || newLevel != userInfo.Level || newMaxScore != userInfo.MaxScore)
            {
                return gameDB.UpdateUserInfoAsync(uid, newLevel, newExp, newMoney, userInfo.Diamond, newMaxScore, userInfo.UserName);
            }

            return Task.CompletedTask;
        }

        public EErrorCode ValidatePlay(GameResultReq req)
        {
            // 플레이 타임 검증
            int cookieId = req.CurrentCookieId;
            int speed = req.Speed;
            if (speed != cookieData[cookieId].Speed)
            {
                return EErrorCode.GameResultService_PlayerSpeedChangedDetected;
            }

            // 아이템 점수, 코인이 조작된 경우
            int score = req.Score;
            int money = req.Money;
            Dictionary<int/*jellyID*/, int/*jellyCount*/>? items = req.Items;
            totalMoney = 0;
            totalScore = 0;
            if (items != null)
            {
                foreach (var pair in items)
                {
                    int itemId = pair.Key;
                    int count = pair.Value;
                    totalScore += itemData[itemId].ScorePoint  * count;
                    totalMoney += itemData[itemId].MoneyPoint * count;
                }

                int expBonus = cookieData[cookieId].ExpBonus;
                int moneyBonus = cookieData[cookieId].MoneyBonus;

                if (expBonus != 0)
                {
                    totalScore += totalScore / 100 * expBonus;
                }

                if (moneyBonus != 0)
                {
                    totalMoney += totalMoney / 100 * moneyBonus;
                }

                if (score != totalScore || money != totalMoney)
                {
                    return EErrorCode.GameResultService_MoneyOrExpChangedDetected;
                }
            }

            return EErrorCode.None;
        }

        public void ReadGameData()
        {
            ReadItemData();
            ReadUserLevelData();
            ReadCookieData();
        }

        public void ReadItemData()
        {
            string filePath = "./Resources/ItemData.csv";
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

                for (int i = 1; i < rows.Count; i++)
                {
                    ItemData item = new ItemData();
                    if (int.TryParse(rows[i][0], out int id)) item.Id = id;
                    item.Name = rows[i][1];
                    item.Type = rows[i][2];
                    if (int.TryParse(rows[i][3], out int score)) item.ScorePoint = score;
                    if (int.TryParse(rows[i][4], out int money)) item.MoneyPoint = money;
                    itemData.Add(id, item);
                }
            }
        }

        public void ReadUserLevelData()
        {
            string filePath = "./Resources/UserLevelData.csv";
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

                for (int i = 1; i < rows.Count; i++)
                {
                    UserLevelData userLevel = new UserLevelData();
                    if (int.TryParse(rows[i][0], out int level)) userLevel.Level = level;
                    if (int.TryParse(rows[i][1], out int exp)) userLevel.MinExp = exp;
                    userLevelData.Add(level, userLevel);
                }
            }
        }

        public void ReadCookieData()
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

                for (int i = 1; i < rows.Count; i++)
                {
                    CookieData cookie = new CookieData();
                    if (int.TryParse(rows[i][0], out int id)) cookie.Id = id;
                    cookie.Name = rows[i][1];
                    cookie.Description = rows[i][2];
                    if (int.TryParse(rows[i][3], out int speed)) cookie.Speed = speed;
                    if (int.TryParse(rows[i][4], out int expBonus)) cookie.ExpBonus = expBonus;
                    if (int.TryParse(rows[i][5], out int MoneyBonus)) cookie.MoneyBonus = MoneyBonus;
                    if (uint.TryParse(rows[i][6], out uint diamondCost)) cookie.DiamondCost = diamondCost;

                    cookieData.Add(id, cookie);
                }
            }
        }
    }
}
