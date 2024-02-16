using System.ComponentModel;

namespace API_Game_Server.Model.DAO
{
    public class FriendRequestElement
    {
        public long RequestId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
}