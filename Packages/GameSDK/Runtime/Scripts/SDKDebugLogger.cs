using System.Runtime.CompilerServices;
#pragma warning disable CS0162 // Unreachable code detected

namespace SDK
{
    /// <summary>
    /// Zero-garbage debug logging system with multiple optimization strategies
    /// </summary>

    public static class SDKDebugLogger
    {
        #region Conditional Compilation Flags
#if CHEAT_ONLY
    private static bool IsShowing => true;
#elif RELEASE_ONLY
    private static bool IsShowing => false;
#else
    private static bool IsShowing => false;
#endif
        #endregion

        #region Zero-Garbage Logging Methods

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {message}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(object message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {message}");
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2, object param3)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2, object param3, object param4)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {message}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(object message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {message}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2, object param3)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2, object param3, object param4)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {message}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(object message)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {message}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2, object param3)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2, object param3, object param4)
        {
            if (!IsShowing) return;
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }

        #endregion
    }
}