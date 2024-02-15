using System.ComponentModel;

namespace API_Game_Server.Model.DAO
{
    public class myName
    {
        public string UserName { get; set; }
    }
    public class FriendInfo
    {
        public string UserName { get; set; }
    }
    public class FriendShipInfo
    {
        public bool IsExist { get; set; }
    }
    public class FriendRequestInfo
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
    public class FriendCountInfo
    {
        public long FriendCount { get; set; }
    }
    public class ReverseFriendShipInfo
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
}