using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    // 요청 데이터
    public class GameResultReq
    {
        // 젤리, 돈, 플레이 시간
        public string Token { get; set; }
        public Dictionary<int/*itemID*/, int/*count*/>? Items { get; set; }
        public int Score { get; set; }
        public int Money { get; set; }
        public int Speed { get; set; }
        public int CurrentCookieId { get; set; }
    }

    // 응답 데이터
    public class GameResultRes : ErrorCodeDTO
    {
        public int Money { get; set; }
        public int Level {  get; set; } 
        public int Exp { get; set; }
    }
}