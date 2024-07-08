using System;
using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Block with multiple components
    /// </summary>
    public class EntityMultipleComponents : VisualElement
    {
        private readonly Dictionary<Type, MultipleComponentDrawer> _componentDrawers = new();
        private Entity _entity;
        private readonly HashSet<Type> _components = new();
        private readonly EditorDrawer _mainDrawer = new();
        private readonly VisualElement _componentsContainer;

        public EntityMultipleComponents()
        {
            var title = new Label("Multiple components");
            title.AddToClassList("modules--one-entity--components-title");
            Add(title);
            _componentsContainer = new();
            Add(_componentsContainer);
        }

        public void SetEntity(Entity entity)
        {
            _entity = entity;
            _components.Clear();
            foreach (var componentType in MF.World.GetEntityMultipleComponentsType(_entity.Id))
                _components.Add(componentType);
        }

        public void Draw()
        {
            _componentsContainer.Clear();
            foreach (var componentType in _components)
                DrawComponents(componentType);
        }

        private void DrawComponents(Type componentType)
        {
            if (!_componentDrawers.TryGetValue(componentType, out var drawer))
            {
                drawer = new MultipleComponentDrawer(componentType, _mainDrawer);
                _componentDrawers[componentType] = drawer;
            }

            drawer.Destroy();
            drawer.SetEntityId(_entity.Id);
            drawer.Draw(_componentsContainer, true);
        }

        public void Destroy()
        {
            foreach (var drawer in _componentDrawers.Values)
                drawer.Destroy();
        }
    }
}