using System;
using System.Collections.Generic;

namespace ModulesFrameworkUnity.EntitiesTags
{
    internal class EntitiesTagStorage
    {
        public static bool IsInitialized => Storage != null;
        public static EntitiesTagStorage Storage { get; private set; }

        private readonly Dictionary<int, SortedSet<string>> _tags = new();

        public event Action<int> OnTagChanged;

        public static void Initialize()
        {
            Storage = new EntitiesTagStorage();
        }

        public void AddTag(int eid, string tag)
        {
            if (!_tags.TryGetValue(eid, out var tags))
            {
                tags = new SortedSet<string>();
                _tags.Add(eid, tags);
            }

            tags.Add(tag);
            OnTagChanged?.Invoke(eid);
        }

        public void RemoveEntity(int eid)
        {
            _tags.Remove(eid);
        }

        public void RemoveTag(int eid, string tag)
        {
            if (!_tags.TryGetValue(eid, out var tags))
                return;
            tags.Remove(tag);
            OnTagChanged?.Invoke(eid);
        }

        public IReadOnlyCollection<string> GetTags(int eid)
        {
            if (!_tags.TryGetValue(eid, out var tags))
                return Array.Empty<string>();
            return tags;
        }
    }
}