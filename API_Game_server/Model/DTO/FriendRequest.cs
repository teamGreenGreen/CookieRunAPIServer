using System.ComponentModel;

namespace API_Game_Server.Controllers.DTO
{
    public class FriendAddReq
    {
        public string FromUserName {  get; set; }
        public string ToUserName { get; set; }
    }
    public class FriendInfo
    {
        public string UserName { get; set; }
    }
    public class FriendShipInfo
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
    public class FriendRequestInfo
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
    public class FriendCountInfo
    {
        public int FriendCount { get; }
    }
    public class ReverseFriendShipInfo
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
}