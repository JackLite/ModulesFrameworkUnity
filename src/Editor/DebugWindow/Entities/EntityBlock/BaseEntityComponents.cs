using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Base class for components block (single and multiple)
    /// </summary>
    public abstract class BaseEntityComponents : VisualElement
    {
        public readonly HashSet<string> pinnedComponents;

        public Action OnPinComponent;

        public BaseEntityComponents(IEnumerable<string> pinnedComponents)
        {
            this.pinnedComponents = pinnedComponents.ToHashSet();
        }

        protected void UpdatePinStatus(Type componentType)
        {
            if (pinnedComponents.Contains(componentType.Name))
            {
                pinnedComponents.Remove(componentType.Name);
                GetDrawer(componentType).SetPinned(false);
            }
            else
            {
                pinnedComponents.Add(componentType.Name);
                GetDrawer(componentType).SetPinned(true);
            }

            Reorder();
            OnPinComponent?.Invoke();
        }

        protected abstract void Reorder();
        protected abstract BaseComponentDrawer GetDrawer(Type componentType);
    }
}