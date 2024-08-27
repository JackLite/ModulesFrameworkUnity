using System;
using Modules.Extensions.Prototypes;
using ModulesFramework.Data;

namespace ModulesFrameworkUnity.EntitiesTags
{
    [Serializable]
    public class EntityPrototypeWithTag : EntityPrototype
    {
        public string tag;

        public override void FillEntity(Entity ent)
        {
            base.FillEntity(ent);
            if (!string.IsNullOrWhiteSpace(tag))
                ent.AddTag(tag);
        }
    }
}