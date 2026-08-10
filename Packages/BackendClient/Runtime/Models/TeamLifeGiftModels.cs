using System;

namespace GameBackendModule.Models
{
    /// <summary>
    /// Thông tin hiển thị của đồng đội đã tặng (tim hoặc thẻ).
    /// </summary>
    [Serializable]
    public class TeamGiftUser
    {
        public string id;
        public string username;
        public int AvatarIndex;
        public int FrameIndex;
        public int BadgeIndex;
        public int EffectNameIndex;
    }

    /// <summary>Model cho endpoint tim đồng đội tặng (<c>/team/{id}/lives/gifts</c>).</summary>
    [Serializable]
    public class TeamLifeGiftData
    {
        public string id;
        public string createdAt;
        public TeamGiftUser helper;
    }

    [Serializable]
    public class TeamLifeGiftsResponse
    {
        public string teamId;
        public string teamName;
        public int teamBadgeId;
        public TeamLifeGiftData[] gifts;
    }

    [Serializable]
    public class ClaimTeamLifeGiftRequest
    {
        public string giftId;
    }

    [Serializable]
    public class ClaimTeamLifeGiftResponse
    {
        public bool claimed;
        public int remaining;
    }
}
