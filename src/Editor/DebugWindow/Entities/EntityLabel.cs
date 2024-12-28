using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.EntitiesTags;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    public class EntityLabel : Label
    {
        public readonly int eid;
        public string displayName;

        public EntityLabel(Entity entity)
        {
            eid = entity.Id;
        }

        public void UpdateName(StringBuilder stringBuilder, bool isFullName)
        {
            stringBuilder.Clear();
            var ent = MF.World.GetEntity(eid);
            var stringId = ent.Id.ToString(CultureInfo.InvariantCulture);
            stringBuilder.Append("[");
            stringBuilder.Append(stringId);
            stringBuilder.Append("] ");
            // try tags
            if (EntitiesTagStorage.IsInitialized)
            {
                var tags = EntitiesTagStorage.Storage.GetTags(eid);
                if (tags.Count > 0)
                {
                    stringBuilder.Append(string.Join(" | ", tags));
                    stringBuilder.Append(" (");
                    stringBuilder.Append(eid.ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append(")");
                    displayName = stringBuilder.ToString();
                    text = displayName;
                    return;
                }
            }

            // try custom id
            var customId = ent.GetCustomId();
            if (customId != stringId)
            {
                stringBuilder.Append(customId);
            }
            else if (isFullName)
            {
                var simpleTypes = MF.World.GetEntitySingleComponentsType(eid).Select(t => t.Name);
                var multipleTypes = MF.World.GetEntityMultipleComponentsType(eid).Select(t => t.Name);
                stringBuilder.Append(string.Join("-", simpleTypes.Union(multipleTypes)));
            }
            else
            {
                stringBuilder.Append("Entity");
            }

            displayName = stringBuilder.ToString();
            text = displayName;
        }
    }
}