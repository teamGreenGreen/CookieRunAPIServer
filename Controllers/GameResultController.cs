using SqlKata;
using SqlKata.Execution;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Humanizer.Localisation;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

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
    private readonly QueryFactory queryFactory;
    private readonly int maxLevel = 3;
    private readonly int maxMoney = 99999;

    public GameResultController(QueryFactory queryFactory)
    {
        this.queryFactory = queryFactory;
        ReadItemData();
        ReadUserLevelData();
    }

    [HttpPost]
    public async Task<ActionResult<GameResultRes>> PostAsync(GameResultReq req)
    {
        // 게임이 종료돼서 클라이언트에서 요청이 들어옴
        string? userId = req.UserId;
        TimeSpan? playTime = req.PlayTime;
        Dictionary<int/*jellyID*/, int/*jellyCount*/>? items = req.Items;

        // Null 값이 있으면 return 함
        if (userId == null || playTime == null) return BadRequest();

        // DB에서 현재 유저 정보를 가져올건데 아이디, 레벨, 경험치, 코인, 최고 점수을 가져올거임
        var userInfo = await queryFactory.Query("ChoAccount")
            .Select("Id", "Level", "Exp", "MoneyPoint", "MaxScore")
            .Where("Id", userId)
            .FirstOrDefaultAsync<TestUserInfo>();

        // 정보 없으면 못찾은거임 -> return
        if (userInfo == null) 
            return NotFound();

        // 찾았으면 플레이 타임 검증
        // 검증은 어떤 식으로 해야하지?

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

        // 만약 레벨 업이 가능하면 레벨 업
        int newMoneyPoint = userInfo.MoneyPoint += totalMoneyPoint;
        int newExp = userInfo.Exp + totalScorePoint;
        int newLevel = userInfo.Level;

        for (int level = newLevel; level <= userLevelData.Count; level++)
        {
            if (newExp < userLevelData[level].MinExp)
            {
                newLevel = level;
                break;
            }
        }

        // 최고 점수 보다 현재 점수가 높으면 최고 점수 갱신
        int newMaxScore = userInfo.MaxScore;
        if(userInfo.MaxScore < totalScorePoint)
        {
            newMaxScore = totalScorePoint;
            // TODO : 준철님 여기서 최고 점수 갱신됩니다. 랭킹 작업 시 참고하세유
        }

        // DB 저장 -> Redis로 수정하기
        if (newMoneyPoint != userInfo.MoneyPoint || newExp != userInfo.Exp || newLevel != userInfo.Level || newMaxScore != userInfo.MaxScore)
        {
            // 업데이트할 데이터
            var dataToUpdate = new
            {
                Level = newLevel,
                Exp = newExp,
                MoneyPoint = newMoneyPoint,
                MaxScore = newMaxScore
            };

            // SQLKata 쿼리 생성
            Query query = new Query("ChoAccount").Where("Id", userInfo.Id).AsUpdate(dataToUpdate);

            try
            {
                // 쿼리 실행
                int affectedRows = await queryFactory.ExecuteAsync(query);

                Console.WriteLine($"Affected Rows: {affectedRows}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to save data: {ex.Message}");
            }
        }

        GameResultRes response = new GameResultRes()
        {
            MoneyPoint = newMoneyPoint,
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

