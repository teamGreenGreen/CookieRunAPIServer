using System.ComponentModel;
using API_Game_Server.Model.DAO;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터

    // 응답 데이터
    public class NowCookieRes : ErrorCodeDTO
    {
        public int NowCookieId { get; set; }
    }
}