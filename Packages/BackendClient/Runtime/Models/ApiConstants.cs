using System;
using UnityEngine;

namespace GameBackendModule.Models
{
    public static class ApiConstants
    {
        public const string BASE_URL = "http://localhost:3000";
        
        // Authentication endpoints
        public const string REGISTER_ENDPOINT = "/api/v1/auth/register";
        public const string LOGIN_ENDPOINT = "/api/v1/auth/login";
        public const string REFRESH_TOKEN_ENDPOINT = "/api/v1/auth/refresh";
        public const string CHANGE_PASSWORD_ENDPOINT = "/api/v1/auth/change-password";
        public const string EXTERNAL_LOGIN_ENDPOINT = "/api/v1/auth/external-login";
        
        // Player endpoints
        public const string PLAYER_PROFILE_ENDPOINT = "/api/v1/player/profile";
        public const string PLAYER_INFO_ENDPOINT = "/api/v1/player/info";
        public const string PLAYER_COUNTRY_ENDPOINT = "/api/v1/player/country";
        public const string PLAYER_SAVE_ENDPOINT = "/api/v1/player/save";
        public const string DELETE_PLAYER_ENDPOINT = "/api/v1/player/{0}";

        /// <summary>POST purge-by-uid — Bearer JWT (chủ tài khoản hoặc admin).</summary>
        public const string USERS_PURGE_BY_UID_ENDPOINT = "/api/v1/users/purge-by-uid";

        // Weekly contest (Bearer JWT)
        public const string WEEKLY_CONTEST_STATUS_ENDPOINT = "/api/v1/weekly-contest/status";
        public const string WEEKLY_CONTEST_CLAIM_ENDPOINT = "/api/v1/weekly-contest/claim";
        public const string WEEKLY_CONTEST_ADD_SCORE_ENDPOINT = "/api/v1/weekly-contest/add-score";
        /// <summary>POST [DEV] ép kết thúc tuần open — Bearer JWT (+ dev key trên production).</summary>
        public const string WEEKLY_CONTEST_DEV_END_WEEK_ENDPOINT = "/api/v1/weekly-contest/dev/end-week";
        public const string WEEKLY_CONTEST_DEV_KEY_HEADER = "X-Weekly-Contest-Dev-Key";

        // Leaderboard (POST submit cần JWT; GET top/rank public)
        public const string LEADERBOARD_SUBMIT_ENDPOINT = "/api/v1/leaderboard/submit";
        public const string LEADERBOARD_TOP_ENDPOINT = "/api/v1/leaderboard/top";
        public const string LEADERBOARD_RANK_ENDPOINT = "/api/v1/leaderboard/rank";
        /// <summary>Mã bảng World trên server (ISO + 'ww').</summary>
        public const string LEADERBOARD_WORLD_COUNTRY_CODE = "ww";
        
        // Team endpoints (Bearer JWT — docs/TEAM_API.md)
        public const string CREATE_TEAM_ENDPOINT = "/api/v1/team";
        public const string MY_TEAM_ENDPOINT = "/api/v1/team/my";
        public const string TEAM_SUGGESTIONS_ENDPOINT = "/api/v1/team/suggestions";
        public const string TEAM_SEARCH_ENDPOINT = "/api/v1/team/search";
        public const string TEAM_DETAILS_ENDPOINT = "/api/v1/team/{0}";
        public const string UPDATE_TEAM_ENDPOINT = "/api/v1/team/{0}";
        public const string TEAM_MEMBERS_ENDPOINT = "/api/v1/team/{0}/members";
        public const string JOIN_TEAM_ENDPOINT = "/api/v1/team/join";
        public const string LEAVE_TEAM_ENDPOINT = "/api/v1/team/leave";
        public const string TRANSFER_TEAM_LEADERSHIP_ENDPOINT = "/api/v1/team/{0}/transfer-leadership";
        public const string KICK_TEAM_MEMBER_ENDPOINT = "/api/v1/team/{0}/kick";
        public const string PROMOTE_TEAM_MEMBER_ENDPOINT = "/api/v1/team/{0}/promote";
        public const string TEAM_JOIN_REQUESTS_ENDPOINT = "/api/v1/team/{0}/join-requests";
        public const string APPROVE_TEAM_JOIN_REQUEST_ENDPOINT = "/api/v1/team/{0}/join-requests/{1}/approve";
        public const string REJECT_TEAM_JOIN_REQUEST_ENDPOINT = "/api/v1/team/{0}/join-requests/{1}/reject";
        public const string TEAM_CHAT_ENDPOINT = "/api/v1/team/{0}/chat";
        public const string TEAM_LIVES_REQUEST_ENDPOINT = "/api/v1/team/{0}/lives/request";
        public const string TEAM_LIVES_HELP_ENDPOINT = "/api/v1/team/{0}/lives/help";
        public const string TEAM_LIVES_STATUS_ENDPOINT = "/api/v1/team/{0}/lives/status";

        // F1 — Ranking + Country
        public const string TEAM_RANKING_ENDPOINT = "/api/v1/team/ranking";
        // F4 — Disband (DELETE /team/{0}, dùng chung path với details)
        public const string DISBAND_TEAM_ENDPOINT = "/api/v1/team/{0}";
        // F2 — Team Gift
        public const string TEAM_GIFTS_ENDPOINT = "/api/v1/team/{0}/gifts";
        public const string TEAM_GIFT_CLAIM_ENDPOINT = "/api/v1/team/{0}/gifts/{1}/claim";
        // F3 — Ask Card
        public const string TEAM_CARD_ASK_ENDPOINT = "/api/v1/team/{0}/cards/ask";
        public const string TEAM_CARD_GIVE_ENDPOINT = "/api/v1/team/{0}/cards/give";
        public const string TEAM_CARD_STATUS_ENDPOINT = "/api/v1/team/{0}/cards/status";

        /// <summary>Level tối thiểu mở khóa Team (client kiểm tra trước khi gọi API).</summary>
        public const int TEAM_UNLOCK_LEVEL = 21;

        /// <summary>Phí tạo team — client tự trừ Coins trước POST /team.</summary>
        public const int TEAM_CREATE_COST_COINS = 100;

        // Clan endpoints
        public const string CREATE_CLAN_ENDPOINT = "/api/v1/clan";
        public const string CLAN_DETAILS_ENDPOINT = "/api/v1/clan/{0}";
        public const string UPDATE_CLAN_ENDPOINT = "/api/v1/clan/{0}";
        public const string JOIN_CLAN_ENDPOINT = "/api/v1/clan/join";
        public const string LEAVE_CLAN_ENDPOINT = "/api/v1/clan/leave";
        public const string KICK_MEMBER_ENDPOINT = "/api/v1/clan/{0}/kick";
        public const string PROMOTE_MEMBER_ENDPOINT = "/api/v1/clan/{0}/promote";
        public const string CLAN_MEMBERS_ENDPOINT = "/api/v1/clan/{0}/members";
        public const string SEARCH_CLAN_ENDPOINT = "/api/v1/clan/search";
        
        // Game endpoints
        public const string START_GAME_ENDPOINT = "/api/v1/game/start";
        
        // HTTP Headers
        public const string AUTHORIZATION_HEADER = "Authorization";
        public const string CONTENT_TYPE_HEADER = "Content-Type";
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string BEARER_PREFIX = "Bearer ";
    }
}
