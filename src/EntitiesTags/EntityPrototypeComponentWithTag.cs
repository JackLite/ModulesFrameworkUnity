using Modules.Extensions.Prototypes;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity.EntitiesTags
{
    public class EntityPrototypeComponentWithTag : EntityPrototypeComponent
    {
        [SerializeField]
        public string tag;

        public override Entity Create()
        {
            return base.Create().AddTag(tag);
        }
    }
}
