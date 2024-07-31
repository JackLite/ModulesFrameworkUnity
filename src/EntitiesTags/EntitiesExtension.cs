using System.Runtime.CompilerServices;
using ModulesFramework.Data;

namespace ModulesFrameworkUnity.EntitiesTags
{
    public static class EntitiesExtension
    {
        /// <summary>
        ///     Add tag to entity. It's used ONLY inside the Unity Editor and do nothing in build.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity AddTag(this Entity entity, string tag)
        {
            #if UNITY_EDITOR
            if (!EntitiesTagStorage.IsInitialized)
            {
                UnityEngine.Debug.LogWarning($"You're trying to add tag {tag} to entity ({entity.Id}). " +
                                             "But storage wasn't initialize");
                return entity;
            }

            EntitiesTagStorage.Storage.AddTag(entity.Id, tag);
            #endif
            return entity;
        }

        /// <summary>
        ///     Remove tag from entity if it exists
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity RemoveTag(this Entity entity, string tag)
        {
            #if UNITY_EDITOR
            if (!EntitiesTagStorage.IsInitialized)
            {
                UnityEngine.Debug.LogWarning($"You're trying to remove tag {tag} from entity ({entity.Id}). " +
                                             "But storage wasn't initialize");
                return entity;
            }

            EntitiesTagStorage.Storage.RemoveTag(entity.Id, tag);
            #endif
            return entity;
        }
    }
}