using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Entities.AddComponent
{
    public class CreateWindowRow : Button
    {
        public Type type;

        private readonly Label _label;

        public event Action<Type> OnChoose;

        public CreateWindowRow()
        {
            _label = new Label();
            Add(_label);

            clicked += () => OnChoose?.Invoke(type);
        }

        public void Init(Type type)
        {
            this.type = type;
            _label.text = type.Name;
            tooltip = type.FullName;
        }
    }
}
