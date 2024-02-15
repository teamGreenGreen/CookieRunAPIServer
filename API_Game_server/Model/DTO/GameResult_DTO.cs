using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    // 요청 데이터
    public class GameResultReq
    {
        // 젤리, 돈, 플레이 시간
        public string Token { get; set; }
        //public TimeSpan PlayTime { get; set; } // TODO: 준철님 사용여부 확인 후 제거하기
        public Dictionary<int/*itemID*/, int/*count*/>? Items { get; set; }
        public uint Score { get; set; }
        public uint Money { get; set; }
        public int Speed { get; set; }
        public int CurrentCookieId { get; set; }
    }

    // 응답 데이터
    public class GameResultRes : ErrorCodeDTO
    {
        //public int Money { get; set; }
        //public int Level { get; set; }
        //public int Exp { get; set; }
        //public int MaxScore { get; set; }
    }
}