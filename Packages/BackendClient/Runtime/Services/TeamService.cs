using System;
using System.Collections;
using GameBackendModule.Models;

namespace GameBackendModule.Services
{
    public interface ITeamService
    {
        /// <summary>POST /team — Bearer JWT. Client tự trừ 100 Coins trước khi gọi.</summary>
        IEnumerator CreateTeam(CreateTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/my — trả null nếu chưa ở team nào.</summary>
        IEnumerator GetMyTeam(Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/suggestions — gợi ý tối đa 20 team.</summary>
        IEnumerator GetSuggestions(Action<ApiResponse<TeamSuggestionsResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/search?query=&amp;page=&amp;limit=</summary>
        IEnumerator SearchTeams(string query, int page, int limit, Action<ApiResponse<TeamSearchResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/:id</summary>
        IEnumerator GetTeamDetails(string teamId, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>PUT /team/:id — Leader only.</summary>
        IEnumerator UpdateTeam(string teamId, UpdateTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/:id/members</summary>
        IEnumerator GetTeamMembers(string teamId, Action<ApiResponse<TeamMemberData[]>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/join — Open: TeamData; Closed: status=pending.</summary>
        IEnumerator JoinTeam(JoinTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/leave</summary>
        IEnumerator LeaveTeam(LeaveTeamRequest request, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/transfer-leadership — Leader only.</summary>
        IEnumerator TransferLeadership(string teamId, TransferTeamLeadershipRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/kick — Leader hoặc CoLeader.</summary>
        IEnumerator KickMember(string teamId, KickTeamMemberRequest request, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/promote — Leader only.</summary>
        IEnumerator PromoteMember(string teamId, PromoteTeamMemberRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/:id/join-requests — Leader/CoLeader.</summary>
        IEnumerator GetJoinRequests(string teamId, Action<ApiResponse<TeamJoinRequestData[]>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/join-requests/:requestId/approve</summary>
        IEnumerator ApproveJoinRequest(string teamId, string requestId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/join-requests/:requestId/reject</summary>
        IEnumerator RejectJoinRequest(string teamId, string requestId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/:id/chat?limit= — newest first.</summary>
        IEnumerator GetChatMessages(string teamId, int limit, Action<ApiResponse<TeamChatMessage[]>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/chat</summary>
        IEnumerator SendChatMessage(string teamId, SendTeamChatRequest request, Action<ApiResponse<TeamChatMessage>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/lives/request — cooldown 4h.</summary>
        IEnumerator RequestLives(string teamId, Action<ApiResponse<TeamLivesRequestResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>POST /team/:id/lives/help — client cộng helperReward.coins.</summary>
        IEnumerator HelpTeammate(string teamId, HelpTeammateRequest request, Action<ApiResponse<TeamLivesHelpResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>GET /team/:id/lives/status</summary>
        IEnumerator GetLivesStatus(string teamId, Action<ApiResponse<TeamLivesStatusResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F1 — GET /team/ranking?scope=world|country&amp;countryCode=&amp;page=&amp;limit=</summary>
        IEnumerator GetRanking(string scope, string countryCode, int page, int limit, Action<ApiResponse<TeamRankingResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F4 — DELETE /team/:id (Leader only) — giải tán team.</summary>
        IEnumerator DisbandTeam(string teamId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F2 — POST /team/:id/gifts — gửi gift cho team.</summary>
        IEnumerator SendGift(string teamId, SendGiftRequest request, Action<ApiResponse<TeamGiftData>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F2 — GET /team/:id/gifts — gift còn hạn + cờ claimed.</summary>
        IEnumerator GetGifts(string teamId, Action<ApiResponse<TeamGiftData[]>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F2 — POST /team/:id/gifts/:giftId/claim — client tự cộng reward từ payload.</summary>
        IEnumerator ClaimGift(string teamId, string giftId, Action<ApiResponse<ClaimGiftResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F3 — POST /team/:id/cards/ask — cooldown 4h riêng.</summary>
        IEnumerator AskCard(string teamId, AskCardRequest request, Action<ApiResponse<AskCardResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F3 — POST /team/:id/cards/give — cho card cho người xin.</summary>
        IEnumerator GiveCard(string teamId, GiveCardRequest request, Action<ApiResponse<GiveCardResponse>> onSuccess, Action<ErrorResponse> onError);

        /// <summary>F3 — GET /team/:id/cards/status</summary>
        IEnumerator GetCardStatus(string teamId, Action<ApiResponse<TeamCardStatusResponse>> onSuccess, Action<ErrorResponse> onError);
    }

    public class TeamService : ITeamService
    {
        private readonly IApiClient apiClient;

        public TeamService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IEnumerator CreateTeam(CreateTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            yield return apiClient.Post(ApiConstants.CREATE_TEAM_ENDPOINT, request, onSuccess, onError);
        }

        public IEnumerator GetMyTeam(Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            yield return apiClient.Get(ApiConstants.MY_TEAM_ENDPOINT, onSuccess, onError);
        }

        public IEnumerator GetSuggestions(Action<ApiResponse<TeamSuggestionsResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            yield return apiClient.Get(ApiConstants.TEAM_SUGGESTIONS_ENDPOINT, onSuccess, onError);
        }

        public IEnumerator SearchTeams(string query, int page, int limit, Action<ApiResponse<TeamSearchResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string q = Uri.EscapeDataString(query ?? string.Empty);
            string endpoint = $"{ApiConstants.TEAM_SEARCH_ENDPOINT}?query={q}&page={page}&limit={limit}";
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator GetTeamDetails(string teamId, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_DETAILS_ENDPOINT, teamId);
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator UpdateTeam(string teamId, UpdateTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.UPDATE_TEAM_ENDPOINT, teamId);
            yield return apiClient.Put(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GetTeamMembers(string teamId, Action<ApiResponse<TeamMemberData[]>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_MEMBERS_ENDPOINT, teamId);
            yield return GetJsonArray(endpoint, TeamJsonHelper.ParseMembers, onSuccess, onError);
        }

        public IEnumerator JoinTeam(JoinTeamRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            yield return apiClient.Post(ApiConstants.JOIN_TEAM_ENDPOINT, request, onSuccess, onError);
        }

        public IEnumerator LeaveTeam(LeaveTeamRequest request, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            yield return apiClient.Post(ApiConstants.LEAVE_TEAM_ENDPOINT, request, onSuccess, onError);
        }

        public IEnumerator TransferLeadership(string teamId, TransferTeamLeadershipRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TRANSFER_TEAM_LEADERSHIP_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator KickMember(string teamId, KickTeamMemberRequest request, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.KICK_TEAM_MEMBER_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator PromoteMember(string teamId, PromoteTeamMemberRequest request, Action<ApiResponse<TeamData>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.PROMOTE_TEAM_MEMBER_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GetJoinRequests(string teamId, Action<ApiResponse<TeamJoinRequestData[]>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_JOIN_REQUESTS_ENDPOINT, teamId);
            yield return GetJsonArray(endpoint, TeamJsonHelper.ParseJoinRequests, onSuccess, onError);
        }

        public IEnumerator ApproveJoinRequest(string teamId, string requestId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.APPROVE_TEAM_JOIN_REQUEST_ENDPOINT, teamId, requestId);
            yield return apiClient.Post(endpoint, new EmptyBody(), onSuccess, onError);
        }

        public IEnumerator RejectJoinRequest(string teamId, string requestId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.REJECT_TEAM_JOIN_REQUEST_ENDPOINT, teamId, requestId);
            yield return apiClient.Post(endpoint, new EmptyBody(), onSuccess, onError);
        }

        public IEnumerator GetChatMessages(string teamId, int limit, Action<ApiResponse<TeamChatMessage[]>> onSuccess, Action<ErrorResponse> onError)
        {
            int safeLimit = UnityEngine.Mathf.Clamp(limit, 1, 100);
            string endpoint = string.Format(ApiConstants.TEAM_CHAT_ENDPOINT, teamId) + "?limit=" + safeLimit;
            yield return GetJsonArray(endpoint, TeamJsonHelper.ParseChatMessages, onSuccess, onError);
        }

        public IEnumerator SendChatMessage(string teamId, SendTeamChatRequest request, Action<ApiResponse<TeamChatMessage>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_CHAT_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator RequestLives(string teamId, Action<ApiResponse<TeamLivesRequestResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_LIVES_REQUEST_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, new EmptyBody(), onSuccess, onError);
        }

        public IEnumerator HelpTeammate(string teamId, HelpTeammateRequest request, Action<ApiResponse<TeamLivesHelpResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_LIVES_HELP_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GetLivesStatus(string teamId, Action<ApiResponse<TeamLivesStatusResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_LIVES_STATUS_ENDPOINT, teamId);
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        // ─── F1: Ranking + Country ────────────────────────────────────────────────
        public IEnumerator GetRanking(string scope, string countryCode, int page, int limit, Action<ApiResponse<TeamRankingResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string s = string.IsNullOrEmpty(scope) ? "world" : scope;
            string endpoint = $"{ApiConstants.TEAM_RANKING_ENDPOINT}?scope={Uri.EscapeDataString(s)}&page={page}&limit={limit}";
            if (!string.IsNullOrEmpty(countryCode))
                endpoint += "&countryCode=" + Uri.EscapeDataString(countryCode);
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        // ─── F4: Disband ──────────────────────────────────────────────────────────
        public IEnumerator DisbandTeam(string teamId, Action<ApiResponse<TeamMessageResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.DISBAND_TEAM_ENDPOINT, teamId);
            yield return apiClient.Delete(endpoint, onSuccess, onError);
        }

        // ─── F2: Team Gift ────────────────────────────────────────────────────────
        public IEnumerator SendGift(string teamId, SendGiftRequest request, Action<ApiResponse<TeamGiftData>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_GIFTS_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GetGifts(string teamId, Action<ApiResponse<TeamGiftData[]>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_GIFTS_ENDPOINT, teamId);
            yield return GetJsonArray(endpoint, TeamJsonHelper.ParseGifts, onSuccess, onError);
        }

        public IEnumerator ClaimGift(string teamId, string giftId, Action<ApiResponse<ClaimGiftResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_GIFT_CLAIM_ENDPOINT, teamId, giftId);
            yield return apiClient.Post(endpoint, new EmptyBody(), onSuccess, onError);
        }

        // ─── F3: Ask Card ─────────────────────────────────────────────────────────
        public IEnumerator AskCard(string teamId, AskCardRequest request, Action<ApiResponse<AskCardResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_CARD_ASK_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GiveCard(string teamId, GiveCardRequest request, Action<ApiResponse<GiveCardResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_CARD_GIVE_ENDPOINT, teamId);
            yield return apiClient.Post(endpoint, request, onSuccess, onError);
        }

        public IEnumerator GetCardStatus(string teamId, Action<ApiResponse<TeamCardStatusResponse>> onSuccess, Action<ErrorResponse> onError)
        {
            string endpoint = string.Format(ApiConstants.TEAM_CARD_STATUS_ENDPOINT, teamId);
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        private IEnumerator GetJsonArray<T>(
            string endpoint,
            Func<string, T> parse,
            Action<ApiResponse<T>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return apiClient.GetRaw(endpoint, (body, statusCode, responseDate) =>
            {
                try
                {
                    onSuccess?.Invoke(new ApiResponse<T>
                    {
                        success = true,
                        message = string.Empty,
                        data = parse(body),
                        statusCode = statusCode,
                        responseDate = responseDate,
                    });
                }
                catch (Exception ex)
                {
                    onError?.Invoke(new ErrorResponse
                    {
                        success = false,
                        message = "Failed to parse array response",
                        error = ex.Message,
                        statusCode = statusCode,
                        responseDate = responseDate,
                    });
                }
            }, onError);
        }
    }
}
