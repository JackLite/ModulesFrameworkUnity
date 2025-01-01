using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    public class EntitiesListSearch : VisualElement
    {
        public event Action<string> OnInputChanged;

        public EntitiesListSearch(string searchString)
        {
            var input = new TextField();
            input.SetValueWithoutNotify(searchString);
            input.AddToClassList("modules--entities-list--search-field");
            input.RegisterValueChangedCallback(ev => OnInputChanged?.Invoke(ev.newValue));
            Add(input);

            var clearBtn = new Button();
            clearBtn.text = "X";
            clearBtn.clicked += () => input.value = string.Empty;
            Add(clearBtn);
        }
    }
}