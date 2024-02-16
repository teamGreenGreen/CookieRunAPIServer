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
        private int newMoneyPoint = 0;
        private int newMaxScore = 0;
        private int newExp = 0;
        private int newLevel = 0;

        public GameResultService(GameDB _gameDB, RedisDB _redisDB, ValidationService _validationService, MailService _mailService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
            mailService = _mailService;
            ReadGameData();
        }

        public async Task<EErrorCode> ValidateResult(GameResultReq req)
        {
            // UID 획득 및 조회
            string stringUid = await validationService.GetUid(req.Token);
            if (stringUid == "") return EErrorCode.InvalidToken;

            long uid = long.Parse(stringUid);

            userinfo = await gameDB.GetUserInfo(uid);
            if (userinfo is null) return EErrorCode.InvalidToken;

            // 플레이어 속도, 플레이 시간 검증
            EErrorCode curError = ValidatePlay(req);
            if (EErrorCode.None != curError) return curError;

            // 경험치, 돈 업데이트
            UpdateExpAndMoney(uid);

            // 최고 기록 업데이트
            await UpdateHighestScore(userinfo);

            // DB 저장
            SaveGameDB(uid);

            return EErrorCode.None;
        }

        public async Task UpdateHighestScore(ResultUserInfo userinfo)
        {
            if (userinfo is null) return;

            newMaxScore = userinfo.MaxScore;
            if (newMaxScore < totalScore)
            {
                // Redis 랭킹 업데이트
                newMaxScore = totalScore;
                await redisDB.SetZset("rank", userinfo.UserName, newMaxScore);
            }
        }

        public async Task<EErrorCode> UpdateExpAndMoney(long uid)
        {
            // 경험치, 돈 추가
            newMoneyPoint = userinfo.Money + totalMoney < maxMoney ? userinfo.Money + totalMoney : maxMoney;
            newExp = userinfo.Exp + totalScore;

            // 레벨업
            newLevel = userinfo.Level;

            for (int level = newLevel; level <= userLevelData.Count; level++)
            {
                if (newExp < userLevelData[level].MinExp)
                {
                    newLevel = level - 1;
                    break;
                }
            }

            if (newLevel != userinfo.Level)
            {
                int rewardCount = 0;
                for (int i = userinfo.Level + 1; i <= newLevel; i++)
                {
                    rewardCount += i * 10;
                }

                DateTime sevenDaysLater = DateTime.Now.AddDays(7);
                await mailService.AddMailList(uid, "운영자", "레벨업 보상", rewardCount, false, "diamond", sevenDaysLater);
            }

            return EErrorCode.None;
        }

        public void SaveGameDB(long uid)
        {
            if (newMoneyPoint != userinfo.Money || newExp != userinfo.Exp || newLevel != userinfo.Level || newMaxScore != userinfo.MaxScore)
            {
                // 업데이트할 데이터
                object dataToUpdate = new
                {
                    level = newLevel,
                    exp = newExp,
                    money = newMoneyPoint,
                    max_score = newMaxScore
                };

                if (dataToUpdate != null)
                    gameDB.ChangeDB(uid, newLevel, newExp, newMoneyPoint, newMaxScore);
            }
        }

        public EErrorCode ValidatePlay(GameResultReq req)
        {
            // 플레이 타임 검증
            int cookieId = req.CurrentCookieId;
            int speed = req.Speed;
            if (speed != cookieData[cookieId].Speed)
            {
                return EErrorCode.PlayerSpeedChangedDetected;
            }

            // 아이템 점수, 코인이 조작된 경우

            uint score = req.Score;
            uint money = req.Money;
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
                    return EErrorCode.MoneyOrExpChangedDetected;
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
