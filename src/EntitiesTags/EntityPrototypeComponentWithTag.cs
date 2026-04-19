using Modules.Extensions.Prototypes;
using ModulesFramework.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace ModulesFrameworkUnity.EntitiesTags
{
    public class EntityPrototypeComponentWithTag : EntityPrototypeComponent
    {
        [FormerlySerializedAs("tag")]
        [SerializeField]
        public string entityTag;

        public override Entity Create()
        {
            return base.Create().AddTag(entityTag);
        }
    }
}
