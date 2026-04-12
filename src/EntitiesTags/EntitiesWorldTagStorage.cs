using System;
using System.Collections.Generic;

namespace ModulesFrameworkUnity.EntitiesTags
{
    internal class EntitiesWorldTagStorage
    {
        private readonly Dictionary<int, SortedSet<string>> _tags = new();

        private readonly int _worldIndex;
        /// <summary>
        ///     Arg 1: Entity ID
        ///     Arg 2: World Index
        /// </summary>
        public event Action<int, int> OnTagChanged;

        public EntitiesWorldTagStorage(int worldIndex)
        {
            _worldIndex = worldIndex;
        }

        public void AddTag(int eid, string tag)
        {
            if (!_tags.TryGetValue(eid, out var tags))
            {
                tags = new SortedSet<string>();
                _tags.Add(eid, tags);
            }

            tags.Add(tag);
            OnTagChanged?.Invoke(eid, _worldIndex);
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
            OnTagChanged?.Invoke(eid, _worldIndex);
        }

        public IReadOnlyCollection<string> GetTags(int eid)
        {
            if (!_tags.TryGetValue(eid, out var tags))
                return Array.Empty<string>();
            return tags;
        }
    }
}