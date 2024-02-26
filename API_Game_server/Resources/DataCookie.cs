namespace API_Game_Server.Resources
{
    public class Cookie
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Speed { get; set; }
        public int ExpBonus { get; set; }
        public int MoneyBonus { get; set; }
        public int DiamondCost { get; set; }
    }

    public class DataCookie
    {
        private static DataCookie _instance;
        public static DataCookie Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataCookie();
                }
                return _instance;
            }
        }

        public Cookie[] Cookies { get; private set; }

        private DataCookie()
        {
            // CSV 데이터를 배열에 추가
            Cookies = new Cookie[]
            {
                new Cookie { ID = 1, Name = "용감한", Description = "기본 쿠키", Speed = 10, ExpBonus = 0, MoneyBonus = 0, DiamondCost = 0 },
                new Cookie { ID = 2, Name = "레몬맛", Description = "레몬 맛이 나는 쿠키", Speed = 10, ExpBonus = 5, MoneyBonus = 0, DiamondCost = 119 },
                new Cookie { ID = 3, Name = "블루베리맛", Description = "블루베리 맛이 나는 쿠키", Speed = 15, ExpBonus = 5, MoneyBonus = 5, DiamondCost = 150 },
                new Cookie { ID = 4, Name = "팬케이크맛", Description = "팬케이크 맛이 나는 쿠키", Speed = 15, ExpBonus = 10, MoneyBonus = 10, DiamondCost = 199 }
            };
        }
    }
}
