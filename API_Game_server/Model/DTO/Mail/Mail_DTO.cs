using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    // 요청 데이터
    public class MailListReq
    {
        public int MailboxId {  get; set; }
        public string Token { get; set; }
    }

    // 응답 데이터
    public class MailListRes : ErrorCodeDTO
    {
    }
}