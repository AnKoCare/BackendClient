using UnityEngine;
using GameBackendModule.Models;

namespace GameBackendModule.Services
{
    /// <summary>
    /// JsonUtility không parse JSON array top-level — bọc tạm thành object.
    /// </summary>
    internal static class TeamJsonHelper
    {
        [System.Serializable]
        private class TeamMemberArrayWrapper
        {
            public TeamMemberData[] items;
        }

        [System.Serializable]
        private class TeamJoinRequestArrayWrapper
        {
            public TeamJoinRequestData[] items;
        }

        [System.Serializable]
        private class TeamChatMessageArrayWrapper
        {
            public TeamChatMessage[] items;
        }

        [System.Serializable]
        private class TeamGiftArrayWrapper
        {
            public TeamGiftData[] items;
        }

        public static TeamMemberData[] ParseMembers(string json)
        {
            return ParseArray<TeamMemberArrayWrapper, TeamMemberData>(json)?.items;
        }

        public static TeamJoinRequestData[] ParseJoinRequests(string json)
        {
            return ParseArray<TeamJoinRequestArrayWrapper, TeamJoinRequestData>(json)?.items;
        }

        public static TeamChatMessage[] ParseChatMessages(string json)
        {
            return ParseArray<TeamChatMessageArrayWrapper, TeamChatMessage>(json)?.items;
        }

        public static TeamGiftData[] ParseGifts(string json)
        {
            return ParseArray<TeamGiftArrayWrapper, TeamGiftData>(json)?.items;
        }

        private static TWrapper ParseArray<TWrapper, TItem>(string json)
            where TWrapper : class
        {
            if (string.IsNullOrWhiteSpace(json) || string.Equals(json.Trim(), "null", System.StringComparison.Ordinal))
                return null;

            string trimmed = json.Trim();
            if (trimmed == "[]")
            {
                var empty = System.Activator.CreateInstance<TWrapper>();
                var field = typeof(TWrapper).GetField("items");
                if (field != null)
                    field.SetValue(empty, System.Array.CreateInstance(typeof(TItem), 0));
                return empty;
            }

            string wrapped = "{\"items\":" + trimmed + "}";
            return JsonUtility.FromJson<TWrapper>(wrapped);
        }
    }
}
