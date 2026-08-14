using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SDK
{
    /// <summary>
    /// Zero-garbage debug logging system.
    ///
    /// Every method is marked [Conditional("SDK_DEBUG")], so when SDK_DEBUG is not defined the
    /// compiler removes the call site *and its arguments* entirely. That distinction matters: a
    /// runtime `if (!enabled) return;` guard still builds the interpolated string at the caller
    /// before the method is entered, so it costs an allocation (plus boxing) on every one of the
    /// ~500 call sites even while logging is off.
    ///
    /// To turn logging back on: add SDK_DEBUG to Player Settings > Scripting Define Symbols.
    /// </summary>
    public static class SDKDebugLogger
    {
        private const string SDK_DEBUG = "SDK_DEBUG";

        #region Zero-Garbage Logging Methods

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"[ADS] {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log($"[ADS] {message}");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1)
        {
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2)
        {
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2, object param3)
        {
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string messageFormat, object param1, object param2, object param3, object param4)
        {
            UnityEngine.Debug.Log($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[ADS] {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError($"[ADS] {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1)
        {
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2)
        {
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2, object param3)
        {
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string messageFormat, object param1, object param2, object param3, object param4)
        {
            UnityEngine.Debug.LogError($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2, object param3)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2, param3)}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string messageFormat, object param1, object param2, object param3, object param4)
        {
            UnityEngine.Debug.LogWarning($"[ADS] {string.Format(messageFormat, param1, param2, param3, param4)}");
        }

        #endregion
    }
}
