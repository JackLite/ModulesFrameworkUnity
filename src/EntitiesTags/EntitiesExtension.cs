using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ModulesFramework.Data;

namespace ModulesFrameworkUnity.EntitiesTags
{
    public static class EntitiesExtension
    {
        /// <summary>
        ///     Add editor tag to entity for debug purpose.
        ///     It's used ONLY inside the Unity Editor and does nothing in build.
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
        ///     Remove editor tag from an entity if it exists. Do nothing in build.
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

        /// <summary>
        ///     Get editor tags from entity. Return an empty array in build.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> GetEntityTags(this Entity entity)
        {
            #if UNITY_EDITOR
            if (!EntitiesTagStorage.IsInitialized)
            {
                UnityEngine.Debug.LogWarning($"You're trying to get tags from entity ({entity.Id}). " +
                                             "But storage wasn't initialize");
                return Array.Empty<string>();
            }

            return EntitiesTagStorage.Storage.GetTags(entity.Id);
            #endif
            return Array.Empty<string>();
        }

        /// <summary>
        ///     Get editor tags string from entity. Return an empty string in build.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEntityTagsAsString(this Entity entity)
        {
            #if UNITY_EDITOR
            return string.Join('|', GetEntityTags(entity));
            #endif
            return string.Empty;
        }
    }
}