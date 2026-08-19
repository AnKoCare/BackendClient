using System;
using System.Collections;
using UnityEngine;
using GameBackendModule.Models;

namespace GameBackendModule.Services
{
    public interface IRoyalLeagueService
    {
        /// <summary>
        /// GET /royal-league/status — Bearer JWT. Một endpoint cho toàn bộ màn hình.
        /// Trả CẢ <c>current</c> (season đang chạy) lẫn <c>pending</c> (kết quả season
        /// đã chốt chưa nhận) — không phải cái này hoặc cái kia.
        /// </summary>
        IEnumerator GetStatus(
            Action<ApiResponse<RoyalLeagueStatusResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>POST /royal-league/join — idempotent. Không bắt buộc gọi trước khi submit Crown.</summary>
        IEnumerator Join(
            string sheetTable,
            bool areaTasksDone,
            Action<ApiResponse<RoyalLeagueJoinResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>
        /// POST /royal-league/crowns — cộng Crown sau khi thắng round.
        /// <paramref name="idempotencyKey"/> phải GIỮ NGUYÊN qua mọi lần retry.
        /// </summary>
        IEnumerator SubmitCrowns(
            string sheetTable,
            int crowns,
            string idempotencyKey,
            Action<ApiResponse<RoyalLeagueSubmitCrownsResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>GET /royal-league/leaderboard — trang đầu 100 entry.</summary>
        IEnumerator GetLeaderboard(
            string sheetTable,
            Action<ApiResponse<RoyalLeagueLeaderboardResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>
        /// GET /royal-league/leaderboard?limit=&amp;offset= — bảng chỉ có tới hạng 1000.
        /// Nên tải lũy tiến: 100 đầu trước, phần còn lại tải nền.
        /// </summary>
        IEnumerator GetLeaderboardPage(
            string sheetTable,
            int limit,
            int offset,
            Action<ApiResponse<RoyalLeagueLeaderboardResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>GET /royal-league/rank — hạng của uid khác. Server có thể trả JSON null.</summary>
        IEnumerator GetRank(
            string sheetTable,
            string uid,
            Action<ApiResponse<RoyalLeagueRankResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>
        /// POST /royal-league/claim — ghi nhận đã nhận thưởng.
        /// Server không trả nội dung thưởng; client tra bảng của mình theo <c>rank</c>.
        /// </summary>
        IEnumerator Claim(
            string sheetTable,
            Action<ApiResponse<RoyalLeagueClaimResponse>> onSuccess,
            Action<ErrorResponse> onError);
    }

    public class RoyalLeagueService : IRoyalLeagueService
    {
        /// <summary>Trần server cho bảng xếp hạng.</summary>
        public const int MaxTopLimit = ApiConstants.ROYAL_LEAGUE_MAX_TOP_LIMIT;
        private const int MinTopLimit = 1;
        private const int DefaultTopLimit = 100;

        private readonly IApiClient apiClient;

        public RoyalLeagueService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        /// <summary>
        /// Sinh khoá chống trùng cho MỘT lần thắng round.
        ///
        /// Quan trọng: gọi ĐÚNG MỘT LẦN lúc thắng round, lưu lại (kèm số Crown) và
        /// dùng lại y nguyên cho mọi lần gửi lại khi mất mạng. Chỉ xoá sau khi
        /// server đã trả 200. Sinh key mới cho mỗi lần retry sẽ khiến Crown bị cộng
        /// nhiều lần — server không có cách nào biết đó là cùng một round.
        /// </summary>
        public static string NewIdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }

        private static ErrorResponse LocalError(string message)
        {
            return new ErrorResponse
            {
                success = false,
                message = message,
                error = message,
                statusCode = 400,
            };
        }

        public IEnumerator GetStatus(
            Action<ApiResponse<RoyalLeagueStatusResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return apiClient.Get(
                ApiConstants.ROYAL_LEAGUE_STATUS_ENDPOINT,
                onSuccess,
                onError);
        }

        public IEnumerator Join(
            string sheetTable,
            bool areaTasksDone,
            Action<ApiResponse<RoyalLeagueJoinResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(sheetTable))
            {
                onError?.Invoke(LocalError("sheetTable is required"));
                yield break;
            }

            var body = new RoyalLeagueJoinRequest
            {
                sheetTable = sheetTable.Trim(),
                areaTasksDone = areaTasksDone,
            };

            yield return apiClient.Post(
                ApiConstants.ROYAL_LEAGUE_JOIN_ENDPOINT,
                body,
                onSuccess,
                onError);
        }

        public IEnumerator SubmitCrowns(
            string sheetTable,
            int crowns,
            string idempotencyKey,
            Action<ApiResponse<RoyalLeagueSubmitCrownsResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(sheetTable))
            {
                onError?.Invoke(LocalError("sheetTable is required"));
                yield break;
            }

            // Chặn ngay ở client: thiếu key thì mọi lần retry đều thành một lần cộng
            // mới. Thà lỗi ồn ào lúc dev còn hơn Crown nhân đôi lúc chạy thật.
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                onError?.Invoke(LocalError(
                    "idempotencyKey is required — dùng RoyalLeagueService.NewIdempotencyKey() " +
                    "MỘT lần cho mỗi round thắng và giữ nguyên khi retry"));
                yield break;
            }

            if (crowns < 1)
            {
                onError?.Invoke(LocalError("crowns must be >= 1"));
                yield break;
            }

            var body = new RoyalLeagueSubmitCrownsRequest
            {
                sheetTable = sheetTable.Trim(),
                idempotencyKey = idempotencyKey.Trim(),
                crowns = crowns,
            };

            yield return apiClient.Post(
                ApiConstants.ROYAL_LEAGUE_CROWNS_ENDPOINT,
                body,
                onSuccess,
                onError);
        }

        public IEnumerator GetLeaderboard(
            string sheetTable,
            Action<ApiResponse<RoyalLeagueLeaderboardResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return GetLeaderboardPage(sheetTable, DefaultTopLimit, 0, onSuccess, onError);
        }

        public IEnumerator GetLeaderboardPage(
            string sheetTable,
            int limit,
            int offset,
            Action<ApiResponse<RoyalLeagueLeaderboardResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(sheetTable))
            {
                // Server cũng bắt buộc field này: đầu season mới có HAI bảng đọc được
                // cùng lúc, thiếu sheetTable là xem nhầm bảng trống của season mới.
                onError?.Invoke(LocalError("sheetTable is required"));
                yield break;
            }

            int safeLimit = Mathf.Clamp(limit, MinTopLimit, MaxTopLimit);
            int safeOffset = Mathf.Clamp(offset, 0, MaxTopLimit - 1);

            string qs = Uri.EscapeDataString(sheetTable.Trim());
            string endpoint =
                $"{ApiConstants.ROYAL_LEAGUE_LEADERBOARD_ENDPOINT}?sheetTable={qs}&limit={safeLimit}&offset={safeOffset}";

            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator GetRank(
            string sheetTable,
            string uid,
            Action<ApiResponse<RoyalLeagueRankResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(sheetTable) || string.IsNullOrWhiteSpace(uid))
            {
                onError?.Invoke(LocalError("sheetTable and uid are required"));
                yield break;
            }

            string qs = Uri.EscapeDataString(sheetTable.Trim());
            string qu = Uri.EscapeDataString(uid.Trim());
            string endpoint =
                $"{ApiConstants.ROYAL_LEAGUE_RANK_ENDPOINT}?sheetTable={qs}&uid={qu}";

            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator Claim(
            string sheetTable,
            Action<ApiResponse<RoyalLeagueClaimResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(sheetTable))
            {
                onError?.Invoke(LocalError("sheetTable is required"));
                yield break;
            }

            var body = new RoyalLeagueClaimRequest { sheetTable = sheetTable.Trim() };

            yield return apiClient.Post(
                ApiConstants.ROYAL_LEAGUE_CLAIM_ENDPOINT,
                body,
                onSuccess,
                onError);
        }
    }
}
