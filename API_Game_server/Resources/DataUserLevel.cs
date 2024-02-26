namespace API_Game_Server.Resources
{
    public class LevelData
    {
        public int Level { get; set; }
        public int MinExp { get; set; }
    }

    public class DataUserLevel
    {
        private static DataUserLevel _instance;
        public static DataUserLevel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataUserLevel();
                }
                return _instance;
            }
        }

        public LevelData[] Levels { get; private set; }

        private DataUserLevel()
        {
            // CSV 데이터를 배열에 추가
            Levels = new LevelData[]
            {
                new LevelData { Level = 1, MinExp = 0 },
                new LevelData { Level = 2, MinExp = 6000000 },
                new LevelData { Level = 3, MinExp = 10000000 },
                new LevelData { Level = 4, MinExp = 30000000 },
                new LevelData { Level = 5, MinExp = 50000000 }
                // 추가 레벨 데이터 계속 추가
            };
        }
    }
}
