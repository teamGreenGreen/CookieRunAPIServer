using API_Game_Server.Model.DAO;
using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    // 요청 데이터
    public class MailReq
    {
        public string Token { get; set; }
        public int MailboxId { get; set; }
    }

    // 응답 데이터
    public class MailRes : ErrorCodeDTO
    {
        public IEnumerable<MailInfo> MailList { get; set; }
    }
}