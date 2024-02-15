public enum EErrorCode
{
    None = 0,
    InvalidToken = 1,

    // Account 1000~1999    
    Create_Account_Fail = 1001,
    Login_Fail = 1002,
    Auth_Fail_InvalidResponse = 1003,

    // Friend 2000~2999
    // 친구 신청 실패
    FriendReqFailSelfRequest = 2000,
    FriendReqFailTargetNotFound = 2001,
    FriendReqFailAlreadyFriend = 2002,
    FriendReqFailAlreadyReqExist = 2003,
    FriendReqFailMyFriendCountExceeded = 2004,
    // 친구 신청 수락 실패
    FriendReqAcceptFailMyFriendCountExceeded = 2005,
    FriendReqAccepyFailTargetFriendCountExceeded = 2006,

    // GameResult 3000~3999
    
    PlayerSpeedChangedDetected = 3000,
    MoneyOrExpChangedDetected = 3001,

}
