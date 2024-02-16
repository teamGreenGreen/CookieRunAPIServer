using System.ComponentModel;

namespace API_Game_Server.Model.DAO
{
    public class RequestInfo
    {
        public long RequestId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
    public class MyFriendCount
    {
        public long FriendCount { get; set; }
    }
    public class TargetFriendCount
    {
        public long FriendCount { get; set; }
    }
}