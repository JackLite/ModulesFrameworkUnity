using System;
using ModulesFramework.Utils.Types;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.StructCreation
{
    public class CreateWindowRow : Button
    {
        private Type _type;

        private readonly Label _label;

        public event Action<Type> OnChoose;

        public CreateWindowRow()
        {
            _label = new Label();
            Add(_label);

            clicked += () => OnChoose?.Invoke(_type);
        }

        public void Init(Type structType)
        {
            _type = structType;
            _label.text = structType.GetTypeName();
            tooltip = structType.Namespace + "." + structType.GetTypeName();
        }
    }
}
