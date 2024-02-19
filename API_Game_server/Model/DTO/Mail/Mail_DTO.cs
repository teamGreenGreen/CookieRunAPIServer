using API_Game_Server.Model.DAO;
using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    // 요청 데이터
    public class MailListReq
    {
        public string Token { get; set; }
    }

    public class MailOpenReq
    {
        public string Token { get; set; }
        public int MailboxId { get; set; }
    }

    // 응답 데이터
    public class MailListRes : ErrorCodeDTO
    {
        public IEnumerable<MailInfo> MailList { get; set; }
    }

    public class MailOpenRes : ErrorCodeDTO
    {
    }
}