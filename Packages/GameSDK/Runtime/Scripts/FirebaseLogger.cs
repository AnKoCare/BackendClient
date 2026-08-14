using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SDK
{
    /// <summary>
    /// Same contract as <see cref="SDKDebugLogger"/>: [Conditional("SDK_DEBUG")] strips the call
    /// site and its arguments, so the interpolated strings below are never built in a release
    /// build. A runtime `if (!IsShowing) return;` guard would still build them at every caller.
    /// </summary>
    public static class FirebaseLogger
    {
        private const string SDK_DEBUG = "SDK_DEBUG";

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"<color=#D84B20>[Firebase]</color> {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.Log($"<color=#D84B20>[Firebase]</color><color=yellow>[Warning]</color> {message}");
        }

        [Conditional(SDK_DEBUG)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message)
        {
            UnityEngine.Debug.Log($"<color=#D84B20>[Firebase]</color><color=red>[Error]</color> {message}");
        }
    }
}
