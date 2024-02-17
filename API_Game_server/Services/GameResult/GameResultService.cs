using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic.FileIO;

namespace API_Game_Server.Services
{
    public class GameResultService
    {
        // GameData
        private readonly Dictionary<int/*ItemID*/, ItemData> itemData = new Dictionary<int, ItemData>();
        private readonly Dictionary<int/*Level*/, UserLevelData> userLevelData = new Dictionary<int, UserLevelData>();
        private readonly Dictionary<int/*CookieID*/, CookieData> cookieData = new Dictionary<int, CookieData>();

        // DB
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;

        // Service
        private readonly ValidationService validationService;
        private readonly MailService mailService;

        // 
        private readonly int maxMoney = 99999;
        private ResultUserInfo userinfo;
        private int totalScore = 0;
        private int totalMoney = 0;

        public GameResultService(GameDB gameDB, RedisDB redisDB, ValidationService validationService, MailService mailService)
        {
            this.gameDB = gameDB;
            this.redisDB = redisDB;
            this.validationService = validationService;
            this.mailService = mailService;
            ReadGameData();
        }

        public async Task<EErrorCode> ValidateRequestAsync(GameResultReq req)
        {
            // UID 얻기
            long uid = -1;
            try
            {
                string stringUid = await validationService.GetUid(req.Token);
                uid = long.Parse(stringUid);
                userinfo = await gameDB.GetUserInfoAsync(uid);
            }
            catch
            {
                return EErrorCode.InvalidToken;
            }

            // 플레이 검증
            return ValidatePlay(req); 
        }

        public async Task<EErrorCode> GiveRewardsAsync(GameResultReq req, GameResultRes res)
        {
            int newMoney = 0;
            int newExp = 0;
            int newLevel = 0;
            int newMaxScore = 0;

            try
            {
                newMoney = CalcMoney();
                newExp = CalcExp();
                newLevel = CalcLevel(newExp);
                newMaxScore = CalcMaxScore();
            }
            catch
            {
                //TODO.김초원 : 게임은 했는데 보상 계산에서 에러난 경우
                return EErrorCode.GameResultService_RewardCalcFail;
            }

            try
            {
                await UpdateUserInfoAsync(userinfo.Uid, newLevel, newExp, newMoney, userinfo.Diamond, newMaxScore, userinfo.UserName);
            }
            catch
            {
                //TODO.김초원 : 유저 정보가 업데이트 되지 않은 경우
                return EErrorCode.GameResultService_UserInfoUpdateError;
            }

            try
            {
                bool bLevelUp = newLevel != userinfo.Level;
                if (bLevelUp)
                    AddReward(newLevel);
            }
            catch
            {
                //TODO.김초원 : 레벨 업은 했는데 보상이 안 나간 경우
                // Log를 남겨 놓고 실제로 레벨 업을 했는데 보상이 안 나간 경우 로그를 확인해서 보상을 해야하나?
                return EErrorCode.GameResultService_AddLevelUpRewardFail;
            }

            // Redis 저장
            await redisDB.SetZset("rank", userinfo.UserName, newMaxScore);

            res.Exp = newExp;
            res.Level = newLevel;
            res.Money = newMoney;

            return EErrorCode.None;
        }

        public int CalcMaxScore()
        {
            int newMaxScore = userinfo.MaxScore;
            if (newMaxScore < totalScore)
            {
                // Redis 랭킹 업데이트
                newMaxScore = totalScore;
            }

            return newMaxScore;
        }

        public void AddReward(int newLevel)
        {
            int rewardCount = 0;
            for (int i = userinfo.Level + 1; i <= newLevel; i++)
            {
                rewardCount += i * 10;
                DateTime sevenDaysLater = DateTime.Now.AddDays(7);
                _ = mailService.AddMailAsync(userinfo.Uid, "운영자", "레벨업 보상", rewardCount, false, "diamond", sevenDaysLater);
            }
        }

        public int CalcMoney()
        {
            return userinfo.Money + totalMoney < maxMoney ? userinfo.Money + totalMoney : maxMoney;
        }

        public int CalcExp()
        {
            return userinfo.Exp + totalScore;
        }

        public int CalcLevel(int newExp)
        {
            int newLevel = userinfo.Level;
            for (int level = newLevel; level <= userLevelData.Count; level++)
            {
                if (newExp < userLevelData[level].MinExp)
                {
                    newLevel = level - 1;
                    break;
                }
            }

            return newLevel;
        }

        public Task UpdateUserInfoAsync(long uid, int newLevel, int newExp, int newMoney, int newDiamond, int newMaxScore, string userName)
        {
            if (newMoney != userinfo.Money || newExp != userinfo.Exp || newLevel != userinfo.Level || newMaxScore != userinfo.MaxScore)
            {
                return gameDB.UpdateUserInfoAsync(uid, newLevel, newExp, newMoney, userinfo.Diamond, newMaxScore, userinfo.UserName);
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
                    totalScore += itemData[itemId].ScorePoint * count;
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
                    if (uint.TryParse(rows[i][7], out uint moneyCost)) cookie.MoneyCost = moneyCost;

                    cookieData.Add(id, cookie);
                }
            }
        }
    }
}
