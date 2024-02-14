using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using API_Game_Server.Repository;

namespace Controllers;

[Route("[controller]")]
[ApiController]
public class GameResultController : ControllerBase
{
    // GameData
    // 현재 GameResult만 사용해서 여기 둠
    public class ItemData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int ScorePoint { get; set; }
        public int MoneyPoint { get; set; }
    }

    public class UserLevelData
    {
        public int Level { get; set; }
        public int MinExp { get; set; }
    }

    private readonly Dictionary<int/*ItemID*/, ItemData> itemData = new Dictionary<int, ItemData>();
    private readonly Dictionary<int/*Level*/, UserLevelData> userLevelData = new Dictionary<int, UserLevelData>();
    private readonly int maxMoney = 99999;
    private readonly int speed = 10;
    private readonly int offset = 1;
    private readonly GameDB gameDB;
    private readonly RedisDB redisDB;

    public GameResultController(GameDB _gameDB, RedisDB _redisDB)
    {
        gameDB = _gameDB;
        redisDB = _redisDB;
        ReadItemData();
        ReadUserLevelData();
    }

    [HttpPost]
    public async Task<ActionResult<GameResultRes>> PostAsync(GameResultReq req)
    {
        // 게임이 종료돼서 클라이언트에서 요청이 들어옴
        int userId = req.UserId;
        TimeSpan? playTime = req.PlayTime;
        Dictionary<int/*jellyID*/, int/*jellyCount*/>? items = req.Items;
        uint score = req.Score;
        uint money = req.Money;
        int xPos = req.XPos;

        // Null 값이 있으면 return 함
        if (userId == null || playTime == null) return BadRequest();

        // DB에서 현재 유저 정보를 가져올건데 아이디, 레벨, 경험치, 코인, 최고 점수을 가져올거임
        TestUserInfo userinfo = await gameDB.GetUserInfo(userId);

        // 정보 없으면 못찾은거임 -> return
        if (userinfo == null)
            return NotFound();

        // 플레이 타임 검증
        int elapsedTime = (int)playTime.Value.TotalSeconds;
        if (xPos > (elapsedTime + offset) * speed)
        {
            return StatusCode(501, $"Unreasonable Player Movement Detected");
        }

        // 경험치 추가, 재화 추가
        int totalScorePoint = 0;
        int totalMoneyPoint = 0;
        foreach (var pair in items)
        {
            int itemId = pair.Key;
            int count = pair.Value;
            totalScorePoint += itemData[itemId].ScorePoint * count;
            totalMoneyPoint += itemData[itemId].MoneyPoint * count;
        }

        if (score != totalScorePoint || money != totalMoneyPoint)
        {
            return StatusCode(501, $"Manipulated the score or money");
        }

        int newMoneyPoint = (userinfo.Money + totalMoneyPoint) < maxMoney ? userinfo.Money + totalMoneyPoint : maxMoney;

        int newExp = userinfo.Exp + totalScorePoint;

        // 만약 레벨 업이 가능하면 레벨 업
        int newLevel = userinfo.Level;

        for (int level = newLevel; level <= userLevelData.Count; level++)
        {
            if (newExp < userLevelData[level].MinExp)
            {
                newLevel = level - 1;
                break;
            }
        }

        // 최고 점수 보다 현재 점수가 높으면 최고 점수 갱신
        int newMaxScore = userinfo.MaxScore;
        if (userinfo.MaxScore < totalScorePoint)
        {
            newMaxScore = totalScorePoint;
            // Redis 랭킹 업데이트
            await redisDB.SetZset("rank", userinfo.UserName, req.Score);
        }

        // DB 저장 -> Redis로 수정하기
        if (newMoneyPoint != userinfo.Money || newExp != userinfo.Exp || newLevel != userinfo.Level || newMaxScore != userinfo.MaxScore)
        {
            // 업데이트할 데이터
            object dataToUpdate = new
            {
                Level = newLevel,
                Exp = newExp,
                Money = newMoneyPoint,
                Max_Score = newMaxScore
            };

            gameDB.ChangeDB(userId, dataToUpdate);
        }

        GameResultRes response = new GameResultRes()
        {
            Money = newMoneyPoint,
            Level = newLevel,
            Exp = newExp,
            MaxScore = newMaxScore
        };

        return response;
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
}

