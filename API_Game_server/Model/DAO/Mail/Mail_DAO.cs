namespace API_Game_Server.Model.DAO
{   
    // DB의 data에 접근하기 위한 객체
    public class MailInfo
    {
        public int MailboxId { get; set; }
        public int Uid { get; set; }
        public bool IsRead { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string RewardType { get; set; }
        public int Count { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime CreatedAt{ get; set; }
    }
}
