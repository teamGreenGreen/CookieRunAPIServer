using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;
using System.Text.Json;

namespace API_Game_Server.Services
{
    public class FriendRequestAcceptService : IFriendRequestAcceptService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validationService;
        public FriendRequestAcceptService(IGameDB _gameDB, IRedisDB _redisDB, IValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<EErrorCode> FriendRequestAccept(long requestId)
        {
            // request_id를 이용해서 from_user_name과 to_user_name 가져오기
            RequestInfo requestInfo = await gameDB.GetRequestInfo(requestId);

            // 나의 친구 수가 최대 친구 수를 넘는지 조회
            string myFriendCountKey = string.Format("friend_relationship:{0}",requestInfo.ToUserName); // 받은 사람 = 나 에 대한 조회
            MyFriendCount myFriendCount = new MyFriendCount();
            myFriendCount.FriendCount = await redisDB.SizeOfSet(myFriendCountKey);
            if(myFriendCount.FriendCount >= 5) // Test를 위해 최대 친구 수 5로 수정 -> 50으로 수정 예정
            {
                return EErrorCode.FriendReqAcceptFailMyFriendCountExceeded;
            }

            // 상대방의 친구 수가 최대 친구 수를 넘는지 조회
            string targetFriendCountKey = string.Format("friend_relationship:{0}",requestInfo.FromUserName); // 보낸 사람 = 상대방 에 대한 조회
            TargetFriendCount targetFriendCount = new TargetFriendCount();
            targetFriendCount.FriendCount = await redisDB.SizeOfSet(targetFriendCountKey);
             if(targetFriendCount.FriendCount >= 5) // Test를 위해 최대 친구 수 5로 수정 -> 50으로 수정 예정
            {
                return EErrorCode.FriendReqAcceptFailTargetFriendCountExceeded;
            }

            // RequestId 에 해당하는 신청의 정보로 FRIEND_RELATIONSHIP 테이블에 등록(mysql + redis)
            string addElementKey1 = string.Format("friend_relationship:{0}",requestInfo.FromUserName);
            string addElementKey2 = string.Format("friend_relationship:{0}",requestInfo.ToUserName);
            await gameDB.InsertFriendShip(requestInfo.FromUserName, requestInfo.ToUserName); // mySql 저장
            await gameDB.InsertFriendShip(requestInfo.ToUserName, requestInfo.FromUserName); // mySql 저장
            await redisDB.AddSetElement(addElementKey1, requestInfo.ToUserName); // redis 저장
            await redisDB.AddSetElement(addElementKey2, requestInfo.FromUserName); // redis 저장

            // RequestId에 해당하는 신청 삭제
            await gameDB.DeleteFriendRequestById(requestId);

            return EErrorCode.None;
        }
    }
}