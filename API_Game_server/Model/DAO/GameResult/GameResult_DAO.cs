namespace API_Game_Server.Model.DAO
{
    // DB의 data에 접근하기 위한 객체
    public class ResultUserInfo
    {
        public int Uid { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Money { get; set; }
        public int Diamond { get; set; }
        public int MaxScore { get; set; }
        public string UserName { get; set; }
    }
}