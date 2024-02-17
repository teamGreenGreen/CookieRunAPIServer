namespace API_Game_Server.Model.DAO
{
    public class RedisUserInfo
    {
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Money { get; set; }
        public int MaxScore { get; set; }
        public int Diamond { get; set; }
    }
}
