public enum EErrorCode
{
    None = 0,
    InvalidToken = 1,
    DontKnow = 2,

    // Account 1000~1999    
    // 인증 관련
    CreateAccountFail = 1001,
    LoginFail = 1002,
    AuthFailInvalidResponse = 1003,
    LoginFailUserNotExist = 1004,
    LoginFailAddRedis = 1005,
    // 캐릭터 생성
    CreateUserFailEmptyNickname = 1006,
    CreateUserFailDuplicateNickname = 1007,
    SessionIdNotProvided = 1008,
    UidNotProvided = 1009,
    SessionIdNotFound = 1010,
    AuthFailWrongSessionId = 1011,

    // Friend 2000~2999
    // 친구 신청 실패
    FriendReqFailSelfRequest = 2000,
    FriendReqFailTargetNotFound = 2001,
    FriendReqFailAlreadyFriend = 2002,
    FriendReqFailAlreadyReqExist = 2003,
    FriendReqFailMyFriendCountExceeded = 2004,
    // 친구 신청 수락 실패
    FriendReqAcceptFailMyFriendCountExceeded = 2005,
    FriendReqAcceptFailTargetFriendCountExceeded = 2006,

    // GameResult 3000~3099
    GameResultService_PlayerSpeedChangedDetected    = 3000,
    GameResultService_MoneyOrExpChangedDetected     = 3001,
    GameResultService_RewardCalcFail                = 3002,
    GameResultService_AddLevelUpRewardFail          = 3003,
    GameResultService_UserInfoUpdateError           = 3004,

    // GameDB_Mail 3100~3199
    MailService_AddFail                              = 3101,
    MailService_RemoveFail                           = 3102,
    MailService_UpdateFail                           = 3103,
    MailService_GetFail                              = 3104,
    MailService_GetListFail                          = 3105,
    MailService_GetInfoFail                          = 3106,
    MailService_RewardFail                           = 3107,

    // GameDB_GameResult 3200~3299
    //GameDB_GetUserInfoFail = 3201,
    //GameDB_UpdateUserInfoFail = 3202,
    //GameDB_UpdateUserInfoFail = 3202,

    // Attendance 4000~4099
    AttendanceCountError = 4000,
    AttendanceFailFindUser = 4001,
    AttendanceFailSetString = 4002,
    AttendanceReqFail = 4003,
    AttendanceUpdateFail = 4004,
    NotExistUserDoingReward = 4005,

    // Rank 4100 ~ 4199
    IsNewbie = 4100,
    RankersNotExist = 4101,
    NoBodyInRanking = 4102,

}
