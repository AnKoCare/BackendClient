using System;
using System.Collections;
using UnityEngine;
using Zenject;
using GameBackendModule.Models;
using GameBackendModule.Services;

namespace GameBackendModule.Examples
{
    /// <summary>
    /// Luồng Royal League đầy đủ, viết ra để làm mẫu cho ba chỗ dễ sai nhất:
    ///
    /// 1. Điều kiện hiện UI KHÔNG phải "đang trong khung thời gian". Hết giờ mà
    ///    người chơi chưa xem kết quả thì vẫn phải hiện event, kể cả người ngoài
    ///    Top 10. Ẩn sớm là Top 10 không bao giờ thấy phần thưởng.
    /// 2. `idempotencyKey` sinh MỘT lần mỗi round thắng, giữ nguyên khi retry.
    /// 3. Khung thời gian lấy từ server (`/status`), không lấy từ Remote Config
    ///    hay giờ máy.
    ///
    /// Bảng thưởng và việc cộng thưởng vào tài khoản là của client — server chỉ
    /// trả thứ hạng và giữ cờ đã nhận.
    /// </summary>
    public class RoyalLeagueExample : MonoBehaviour
    {
        [Inject] private IRoyalLeagueService royalLeague;

        [Header("Đọc từ Remote Config: Seasons[].SheetTable")]
        [SerializeField] private string sheetTable = "AntLeagueS1";

        [Header("Client tự kiểm tra, server không verify")]
        [SerializeField] private bool hasMaxLevel;
        [SerializeField] private bool areaTasksDone;

        private RoyalLeagueStatusResponse status;

        // Người ngoài Top 10 không có gì để claim trên server, nên client tự nhớ
        // là đã xem kết quả rồi. Mất save thì cùng lắm xem lại một lần — vô hại.
        private const string SeenResultPrefKey = "rl_seen_result_";

        private void Start()
        {
            StartCoroutine(RefreshStatus());
        }

        // ─── 1. Trạng thái ───────────────────────────────────────────────────

        private IEnumerator RefreshStatus()
        {
            yield return royalLeague.GetStatus(
                res =>
                {
                    status = res.data;
                    ApplyUiState();
                },
                err => Debug.LogError($"[RoyalLeague] status lỗi: {err.statusCode} {err.message}"));
        }

        /// <summary>
        /// Quyết định hiện gì. Đây là chỗ dễ sai nhất của cả tính năng.
        /// </summary>
        private void ApplyUiState()
        {
            if (status == null)
            {
                HideEvent();
                return;
            }

            bool seasonRunning =
                status.current != null && status.current.status == "active";

            if (seasonRunning)
            {
                // Điều kiện vào League do CLIENT kiểm tra — server không có dữ liệu
                // level/area và cũng không chặn.
                if (hasMaxLevel && areaTasksDone)
                {
                    ShowLeagueButton(status.current);
                }
                else
                {
                    HideEvent(); // chưa phá hết level → chơi level thường trước
                }
                return;
            }

            // Hết giờ rồi, NHƯNG còn kết quả chưa xem thì vẫn phải hiện.
            if (status.pending != null && !HasSeenResult(status.pending.sheetTable))
            {
                ShowResultPopup(status.pending);
                return;
            }

            HideEvent();
        }

        // ─── 2. Cộng Crown sau khi thắng round ───────────────────────────────

        /// <summary>
        /// Gọi khi người chơi thắng một round.
        ///
        /// `key` phải được sinh MỘT lần ở đây rồi lưu lại cùng số Crown. Nếu gửi
        /// hỏng vì mạng, retry phải dùng LẠI đúng key đó — server nhận diện và
        /// không cộng lần hai. Sinh key mới mỗi lần retry là Crown bị nhân lên.
        /// </summary>
        public void OnRoundWon(int crowns)
        {
            string key = RoyalLeagueService.NewIdempotencyKey();
            PendingSubmitStore.Save(key, crowns); // lưu để retry được sau khi app tắt
            StartCoroutine(SubmitCrowns(key, crowns));
        }

        private IEnumerator SubmitCrowns(string idempotencyKey, int crowns)
        {
            yield return royalLeague.SubmitCrowns(
                sheetTable,
                crowns,
                idempotencyKey,
                res =>
                {
                    PendingSubmitStore.Clear(idempotencyKey);

                    // Dùng số server trả về để ghi đè, đừng tự cộng dồn ở client:
                    // server có thể đã cắt trần, hoặc đây là lần gửi lại.
                    Debug.Log(
                        $"[RoyalLeague] crowns={res.data.crowns} (+{res.data.crownsGained}) " +
                        $"rank={res.data.rank} duplicate={res.data.duplicate}");
                },
                err => HandleSubmitError(err, idempotencyKey));
        }

        private void HandleSubmitError(ErrorResponse err, string idempotencyKey)
        {
            switch (err.code)
            {
                case ApiConstants.RL_ERR_SEASON_CLOSED:
                    // Season vừa đóng giữa lúc chơi. Crown này mất, không retry nữa.
                    PendingSubmitStore.Clear(idempotencyKey);
                    StartCoroutine(RefreshStatus());
                    break;

                case ApiConstants.RL_ERR_UNKNOWN_SEASON:
                    // Remote Config và server đang lệch nhau — báo để còn biết.
                    Debug.LogError($"[RoyalLeague] sheetTable '{sheetTable}' không có trên server");
                    PendingSubmitStore.Clear(idempotencyKey);
                    break;

                case ApiConstants.RL_ERR_RATE_LIMITED:
                    // Giữ key lại, thử lại sau. Không sinh key mới.
                    Debug.LogWarning("[RoyalLeague] gửi quá nhanh, sẽ thử lại sau");
                    break;

                default:
                    // Mạng hỏng / 5xx — giữ key để lần mở app sau gửi lại.
                    Debug.LogWarning($"[RoyalLeague] submit lỗi {err.statusCode}, sẽ retry: {err.message}");
                    break;
            }
        }

        // ─── 3. Bảng xếp hạng ────────────────────────────────────────────────

        /// <summary>
        /// Tải lũy tiến: 100 hạng đầu hiện ngay, phần còn lại tải nền.
        /// Gọi thẳng limit=1000 lúc cao điểm là chỗ duy nhất server bị chậm.
        /// </summary>
        public IEnumerator LoadLeaderboard(
            Action<RoyalLeagueLeaderboardEntry[]> onFirstPage,
            Action<RoyalLeagueLeaderboardEntry[]> onRest)
        {
            yield return royalLeague.GetLeaderboardPage(
                sheetTable, 100, 0,
                res =>
                {
                    onFirstPage?.Invoke(res.data.entries);
                    if (res.data.boardTotal > 100)
                    {
                        StartCoroutine(royalLeague.GetLeaderboardPage(
                            sheetTable, res.data.boardTotal - 100, 100,
                            more => onRest?.Invoke(more.data.entries),
                            err => Debug.LogWarning($"[RoyalLeague] tải nền lỗi: {err.message}")));
                    }
                },
                err => Debug.LogError($"[RoyalLeague] leaderboard lỗi: {err.message}"));
        }

        // ─── 4. Nhận thưởng ──────────────────────────────────────────────────

        /// <summary>Gọi khi người chơi bấm nút nhận trên popup kết quả.</summary>
        public void OnClaimPressed()
        {
            if (status?.pending == null)
            {
                return;
            }
            StartCoroutine(Claim(status.pending.sheetTable));
        }

        private IEnumerator Claim(string season)
        {
            yield return royalLeague.Claim(
                season,
                res =>
                {
                    MarkResultSeen(season);

                    // alreadyClaimed = true nghĩa là lần trước đã cộng rồi —
                    // KHÔNG được cộng thưởng lần nữa.
                    if (res.data.isWinner && res.data.eligible && !res.data.alreadyClaimed)
                    {
                        GrantReward(res.data.rank);
                    }

                    StartCoroutine(RefreshStatus());
                },
                err => Debug.LogError($"[RoyalLeague] claim lỗi: {err.code} {err.message}"));
        }

        /// <summary>
        /// Tra bảng thưởng của CLIENT (đọc từ SheetTable trên Remote Config) theo
        /// thứ hạng, rồi cộng Coin / Booster / danh hiệu vào tài khoản.
        /// Server không biết phần thưởng là gì.
        /// </summary>
        private void GrantReward(int rank)
        {
            Debug.Log($"[RoyalLeague] cộng thưởng cho hạng {rank} — nối vào hệ thống thưởng của game");
        }

        // ─── UI hooks (nối vào màn hình thật) ────────────────────────────────

        private void ShowLeagueButton(RoyalLeagueCurrentSeason season)
        {
            Debug.Log(
                $"[RoyalLeague] hiện nút League — crowns={season.me.crowns} " +
                $"rank #{season.me.rank} (top {season.me.percentile}%) / {season.totalParticipants} người");
        }

        private void ShowResultPopup(RoyalLeaguePendingResult result)
        {
            Debug.Log(
                $"[RoyalLeague] popup kết quả — hạng {result.rank}/{result.totalParticipants}, " +
                $"{result.crowns} crown, isWinner={result.isWinner}");
        }

        private void HideEvent()
        {
            Debug.Log("[RoyalLeague] ẩn event, về map thường");
        }

        // ─── Ghi nhớ đã xem kết quả (chỉ cho người ngoài Top 10) ─────────────

        private static bool HasSeenResult(string season)
        {
            return PlayerPrefs.GetInt(SeenResultPrefKey + season, 0) == 1;
        }

        private static void MarkResultSeen(string season)
        {
            PlayerPrefs.SetInt(SeenResultPrefKey + season, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Lưu các lần submit chưa được server xác nhận, để gửi lại sau khi app mở lại.
    ///
    /// Đây là nửa còn lại của cơ chế chống trùng: server đảm bảo cùng một key chỉ
    /// cộng một lần, nhưng chỉ có tác dụng nếu client thật sự giữ lại key đó thay
    /// vì sinh mới.
    /// </summary>
    public static class PendingSubmitStore
    {
        private const string KeyList = "rl_pending_keys";

        public static void Save(string idempotencyKey, int crowns)
        {
            PlayerPrefs.SetInt($"rl_pending_{idempotencyKey}", crowns);
            string list = PlayerPrefs.GetString(KeyList, "");
            PlayerPrefs.SetString(KeyList, string.IsNullOrEmpty(list) ? idempotencyKey : list + "," + idempotencyKey);
            PlayerPrefs.Save();
        }

        public static void Clear(string idempotencyKey)
        {
            PlayerPrefs.DeleteKey($"rl_pending_{idempotencyKey}");
            string list = PlayerPrefs.GetString(KeyList, "");
            PlayerPrefs.SetString(KeyList, list.Replace(idempotencyKey, "").Replace(",,", ",").Trim(','));
            PlayerPrefs.Save();
        }

        /// <summary>Các key còn treo — gọi lúc mở app để gửi lại.</summary>
        public static string[] Pending()
        {
            string list = PlayerPrefs.GetString(KeyList, "");
            return string.IsNullOrEmpty(list) ? Array.Empty<string>() : list.Split(',');
        }

        public static int CrownsOf(string idempotencyKey)
        {
            return PlayerPrefs.GetInt($"rl_pending_{idempotencyKey}", 0);
        }
    }
}
