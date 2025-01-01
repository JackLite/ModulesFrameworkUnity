using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Filtering entities by name of component
    /// </summary>
    public class EntitiesNameFilter : VisualElement
    {
        private TextField _input;

        public event Action<string> OnInputChanged;

        public void Draw(string filterString)
        {
            _input = new TextField();
            _input.label = "Component name";
            _input.SetValueWithoutNotify(filterString);
            _input.AddToClassList("modules--entities-tab--name-filter");
            _input.RegisterValueChangedCallback(ev => OnInputChanged?.Invoke(ev.newValue));
            Add(_input);

            var clearBtn = new Button();
            clearBtn.text = "X";
            clearBtn.clicked += () => _input.value = string.Empty;
            Add(clearBtn);
        }
    }
}