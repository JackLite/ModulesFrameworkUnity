using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities.AddComponent
{
    public class AddComponentOneRow : Button
    {
        public Type type;

        private readonly Label _label;

        public event Action<Type> OnChoose;

        public AddComponentOneRow()
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
