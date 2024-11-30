using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities.EntityBlock
{
    /// <summary>
    ///     Block with buttons for entity
    /// </summary>
    public class EntityButtons : VisualElement
    {
        public event Action OnAddComponentClick;
        public event Action OnDestroyClick;

        public void Draw()
        {
            var addComponentBtn = new Button();
            addComponentBtn.text = "Add component";
            addComponentBtn.AddToClassList("modules--entities-tab--entity-buttons--add-component-button");
            addComponentBtn.clicked += () => OnAddComponentClick?.Invoke();
            Add(addComponentBtn);

            var deleteEntityBtn = new Button();
            deleteEntityBtn.text = "Destroy entity";
            deleteEntityBtn.AddToClassList("modules--entities-tab--entity-buttons--destroy-entity-button");
            deleteEntityBtn.clicked += () => OnDestroyClick?.Invoke();
            Add(deleteEntityBtn);
        }
    }
}
