using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;

namespace API_Game_Server.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendRequestService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<EErrorCode> FriendRequest(string sessionId, string toUserName)
        {
            // 가져온 sessionId 이용해서 클라이언트의 user_name 조회
            string sessionIdKey = string.Format("user_info:session_id:{0}",sessionId);
            string userInfo = await redisDB.GetString(sessionIdKey);
            // 가져온 string 역직렬화
            UserInfo myInfo = JsonSerializer.Deserialize<UserInfo>(userInfo);
            // 역직렬화한 string에서 user_name 프로퍼티만 가져오기
            string myName = myInfo.UserName;

            // 자기 자신의 이름인지 확인
            if (myName == toUserName) // 보내는 username과 받는 username이 동일하면 자기가 자기한테 친구 신청
            {
                return EErrorCode.FriendReqFailSelfRequest;
            }

            // toUserName의 유저가 존재하는지 확인
            FriendInfo myFriendInfo = await gameDB.GetFriendInfo(toUserName);
            if(myFriendInfo == null)
            {
                return EErrorCode.FriendReqFailTargetNotFound;
            }

            // 이미 친구인지 확인
            string friendshipKey = string.Format("friend_relationship:{0}",myName);
            FriendShipInfo friendshipInfo = new FriendShipInfo();
            friendshipInfo.IsExist = await redisDB.GetSetIsMemberExist(friendshipKey, toUserName);
            if(friendshipInfo.IsExist)
            {
                return EErrorCode.FriendReqFailAlreadyFriend;
            }

            // 동일한 요청 존재하는지 확인
            FriendRequestInfo friendRequestInfo = await gameDB.GetFriendRequestInfo(myName, toUserName);
            if(friendRequestInfo != null)
            {
                return EErrorCode.FriendReqFailAlreadyReqExist;
            }

            // 최대 친구 수 초과 확인
            string myFriendCountKey = string.Format("friend_relationship:{0}",myName);
            FriendCountInfo myFriendCount = new FriendCountInfo();
            myFriendCount.FriendCount = await redisDB.SizeOfSet(myFriendCountKey);
            if(myFriendCount.FriendCount >= 5) // Test를 위해 최대 친구 수 5로 수정 -> 나중에 50으로 바꿀 예정
            {
                return EErrorCode.FriendReqFailMyFriendCountExceeded;
            }

            // 역방향 신청 존재하는지 확인
            ReverseRequestInfo reverseRequestInfo = await gameDB.GetReverseRequestInfo(myName, toUserName);
            if(reverseRequestInfo != null)
            {
                string addElementKey1 = string.Format("friend_relationship:{0}",myName);
                string addElementKey2 = string.Format("friend_relationship:{0}",toUserName);
                await gameDB.InsertFriendShip(myName,toUserName); // mySql 저장
                await gameDB.InsertFriendShip(toUserName,myName); // mySql 저장
                await redisDB.AddSetElement(addElementKey1, toUserName); // redis 저장
                await redisDB.AddSetElement(addElementKey2, myName); // redis 저장
                await gameDB.DeleteFriendRequest(toUserName,myName); // 역방향 신청 삭제
                return EErrorCode.None;
            }

            // 신청이 완료된 경우
            await gameDB.InsertFriendRequest(myName, toUserName); // DB에 작성
            return EErrorCode.None;
        }
    }
}