using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Utils.Types;
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
            if (pinnedComponents.Contains(componentType.GetTypeName()))
            {
                pinnedComponents.Remove(componentType.GetTypeName());
                GetDrawer(componentType).SetPinned(false);
            }
            else
            {
                pinnedComponents.Add(componentType.GetTypeName());
                GetDrawer(componentType).SetPinned(true);
            }

            Reorder();
            OnPinComponent?.Invoke();
        }

        protected abstract void Reorder();
        protected abstract BaseComponentDrawer GetDrawer(Type componentType);
    }
}