using System;
using System.Collections.Generic;

namespace ModulesFrameworkUnity.EntitiesTags
{
    internal class EntitiesTagStorage
    {
        public static bool IsInitialized => Storage != null;
        public static EntitiesTagStorage Storage { get; private set; }

        private readonly Dictionary<int, EntitiesWorldTagStorage> _worldStorages = new();

        /// <summary>
        ///     Arg 1: Entity ID
        ///     Arg 2: World Index
        /// </summary>
        public event Action<int, int> OnTagChanged;

        public static void Initialize()
        {
            Storage = new EntitiesTagStorage();
        }

        public void AddTag(int eid, int worldIndex, string tag)
        {
            EnsureWorldStorage(worldIndex);
            var storage = _worldStorages[worldIndex];
            storage.AddTag(eid, tag);
        }

        private void EnsureWorldStorage(int worldIndex)
        {
            if (!_worldStorages.ContainsKey(worldIndex))
            {
                var storage = new EntitiesWorldTagStorage(worldIndex);
                storage.OnTagChanged += (eid, idx) => OnTagChanged?.Invoke(eid, idx);
                _worldStorages.Add(worldIndex, storage);
            }
        }

        public void RemoveEntity(int eid, int worldIndex)
        {
            EnsureWorldStorage(worldIndex);
            var storage = _worldStorages[worldIndex];
            storage.RemoveEntity(eid);
        }

        public void RemoveTag(int eid, int worldIndex, string tag)
        {
            EnsureWorldStorage(worldIndex);
            var storage = _worldStorages[worldIndex];
            storage.RemoveTag(eid, tag);
        }

        public IReadOnlyCollection<string> GetTags(int eid, int worldIndex)
        {
            EnsureWorldStorage(worldIndex);
            var storage = _worldStorages[worldIndex];
            return storage.GetTags(eid);
        }
    }
}