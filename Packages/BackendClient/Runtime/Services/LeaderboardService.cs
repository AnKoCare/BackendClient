using System;
using System.Collections;
using UnityEngine;
using GameBackendModule.Models;

namespace GameBackendModule.Services
{
    public interface ILeaderboardService
    {
        /// <summary>POST /leaderboard/submit — cần Bearer JWT. Ghi đè điểm hiện tại của player.</summary>
        IEnumerator SubmitScore(
            LeaderboardSubmitScoreRequest request,
            Action<ApiResponse<LeaderboardSubmitScoreResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>GET /leaderboard/top?countryCode= — public. countryCode = ww (World) hoặc ISO alpha-2 thường. Server trả top 100.</summary>
        IEnumerator GetTop(
            string countryCode,
            Action<ApiResponse<LeaderboardTopResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>
        /// GET /leaderboard/top?countryCode=&amp;limit=&amp;offset= — public.
        /// Board chỉ có tới hạng 1000: <paramref name="limit"/> 1..1000, <paramref name="offset"/> 0..999.
        /// <c>data.total</c> = số entry trả về; <c>data.boardTotal</c> = tổng entry của board (0 nếu server cũ).
        /// </summary>
        IEnumerator GetTopPage(
            string countryCode,
            int limit,
            int offset,
            Action<ApiResponse<LeaderboardTopResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>GET /leaderboard/rank (query uid + countryCode) — public. Server có thể trả JSON null.</summary>
        IEnumerator GetRank(
            string uid,
            string countryCode,
            Action<ApiResponse<LeaderboardRankResponse>> onSuccess,
            Action<ErrorResponse> onError);
    }

    public class LeaderboardService : ILeaderboardService
    {
        /// <summary>Trần server cho <c>GET /leaderboard/top</c> (board chỉ tính tới hạng 1000).</summary>
        public const int MaxTopLimit = 1000;
        private const int MinTopLimit = 1;

        private readonly IApiClient apiClient;

        public LeaderboardService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IEnumerator SubmitScore(
            LeaderboardSubmitScoreRequest request,
            Action<ApiResponse<LeaderboardSubmitScoreResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return apiClient.Post(ApiConstants.LEADERBOARD_SUBMIT_ENDPOINT, request, onSuccess, onError);
        }

        public IEnumerator GetTop(
            string countryCode,
            Action<ApiResponse<LeaderboardTopResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                onError?.Invoke(new ErrorResponse
                {
                    success = false,
                    message = "countryCode is required",
                    error = "countryCode is required",
                    statusCode = 400,
                });
                yield break;
            }

            string q = Uri.EscapeDataString(countryCode.Trim().ToLowerInvariant());
            string endpoint = $"{ApiConstants.LEADERBOARD_TOP_ENDPOINT}?countryCode={q}";
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator GetTopPage(
            string countryCode,
            int limit,
            int offset,
            Action<ApiResponse<LeaderboardTopResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                onError?.Invoke(new ErrorResponse
                {
                    success = false,
                    message = "countryCode is required",
                    error = "countryCode is required",
                    statusCode = 400,
                });
                yield break;
            }

            // Server chỉ chấp nhận limit 1..1000 và offset 0..999 — clamp phía client
            // để tham số lệch không biến thành lỗi 400.
            int safeLimit = Mathf.Clamp(limit, MinTopLimit, MaxTopLimit);
            int safeOffset = Mathf.Clamp(offset, 0, MaxTopLimit - 1);

            string q = Uri.EscapeDataString(countryCode.Trim().ToLowerInvariant());
            string endpoint =
                $"{ApiConstants.LEADERBOARD_TOP_ENDPOINT}?countryCode={q}&limit={safeLimit}&offset={safeOffset}";
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }

        public IEnumerator GetRank(
            string uid,
            string countryCode,
            Action<ApiResponse<LeaderboardRankResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(countryCode))
            {
                onError?.Invoke(new ErrorResponse
                {
                    success = false,
                    message = "uid and countryCode are required",
                    error = "uid and countryCode are required",
                    statusCode = 400,
                });
                yield break;
            }

            string qu = Uri.EscapeDataString(uid.Trim());
            string qc = Uri.EscapeDataString(countryCode.Trim().ToLowerInvariant());
            string endpoint = $"{ApiConstants.LEADERBOARD_RANK_ENDPOINT}?uid={qu}&countryCode={qc}";
            yield return apiClient.Get(endpoint, onSuccess, onError);
        }
    }
}
