using System;

namespace GameBackendModule.Models
{
    /// <summary>
    /// Model cho endpoint thẻ đồng đội cho (<c>/team/{id}/cards/gifts</c>).
    /// Server chỉ giữ entry, thẻ thật do client cộng vào collection khi claim.
    /// </summary>
    [Serializable]
    public class TeamCardGiftData
    {
        public string id;
        public int cardIndex;
        public string createdAt;
        public TeamGiftUser giver;
    }

    [Serializable]
    public class TeamCardGiftsResponse
    {
        public string teamId;
        public string teamName;
        public int teamBadgeId;
        public TeamCardGiftData[] gifts;
    }

    [Serializable]
    public class ClaimTeamCardGiftRequest
    {
        public string giftId;
    }

    [Serializable]
    public class ClaimTeamCardGiftResponse
    {
        public bool claimed;
        public int remaining;
    }
}
