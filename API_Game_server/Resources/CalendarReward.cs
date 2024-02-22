namespace API_Game_Server.Resources
{
    public class CalendarReward
    {
        private static CalendarReward _instance;
        public static CalendarReward Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CalendarReward();
                }
                return _instance;
            } 
        }
        public string[] rewards = new string[] {"diamond:5", "money:5000", "diamond:4", "money:5000", "diamond:2", "money:5000", "diamond:10", "diamond:5", "money:5000", "diamond:4", "money:5000", "diamond:2", "money:5000", "diamond:10", "diamond:5", "money:5000", "diamond:4", "money:5000", "diamond:10", };
    }
}
