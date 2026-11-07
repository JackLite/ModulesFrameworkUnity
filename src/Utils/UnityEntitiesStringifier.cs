using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.EntitiesTags;

namespace ModulesFrameworkUnity.Utils
{
    public class UnityEntitiesStringifier : IEntityStringifier
    {
        public string ToString(Entity entity)
        {
            var tags = entity.GetEntityTagsAsString();
            if (string.IsNullOrEmpty(tags))
                return "Entity " + entity.GetCustomId();

            return $"[{entity.Id}] {tags}";
        }
    }
}
