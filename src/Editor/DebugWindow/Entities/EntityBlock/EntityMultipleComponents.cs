using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Block with multiple components
    /// </summary>
    public class EntityMultipleComponents : BaseEntityComponents
    {
        private readonly Dictionary<Type, MultipleComponentDrawer> _componentDrawers = new();
        private Entity _entity;
        private readonly HashSet<Type> _components = new();
        private readonly EditorDrawer _mainDrawer = new();
        private readonly VisualElement _componentsContainer;
        private readonly HashSet<Type> _old = new();
        private readonly HashSet<Type> _new = new();
        private bool _isAllOpen;

        public EntityMultipleComponents(IEnumerable<string> pinnedComponents) : base(pinnedComponents)
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
            foreach (var componentType in DebugUtils.GetCurrentWorld().GetEntityMultipleComponentsType(_entity.Id))
                _components.Add(componentType);
        }

        public void Draw(bool isAllOpen)
        {
            _isAllOpen = isAllOpen;
            _componentsContainer.Clear();
            foreach (var componentType in _components)
                DrawComponents(componentType);
            Reorder();
        }

        public void OnEntityChanged()
        {
            if (!_entity.IsAlive())
                return;

            _old.Clear();
            _new.Clear();
            _old.UnionWith(_components);
            foreach (var componentType in DebugUtils.GetCurrentWorld().GetEntityMultipleComponentsType(_entity.Id))
            {
                if (!_old.Contains(componentType))
                    _new.Add(componentType);
                _old.Remove(componentType);
            }

            foreach (var oldComponent in _old)
            {
                _componentDrawers[oldComponent].Destroy();
                _components.Remove(oldComponent);
            }

            foreach (var componentType in _new)
            {
                DrawComponents(componentType);
                _components.Add(componentType);
            }

            foreach (var (_, drawer) in _componentDrawers)
                drawer.OnEntityChanged();

            Reorder();
        }

        protected override void Reorder()
        {
            var ordered = _componentDrawers
                .OrderBy(kvp => pinnedComponents.Contains(kvp.Key.Name))
                .ThenByDescending(kvp => kvp.Key.Name);

            foreach (var (_, drawer) in ordered)
                drawer.SetFirst();
        }

        protected override BaseComponentDrawer GetDrawer(Type componentType)
        {
            return _componentDrawers.GetValueOrDefault(componentType);
        }

        private void DrawComponents(Type componentType)
        {
            if (!_componentDrawers.TryGetValue(componentType, out var drawer))
            {
                drawer = new MultipleComponentDrawer(componentType, _mainDrawer);
                _componentDrawers[componentType] = drawer;
                drawer.OnPin += () => UpdatePinStatus(componentType);
                drawer.SetPinned(pinnedComponents.Contains(componentType.GetTypeName()));
            }

            drawer.Destroy();
            drawer.SetEntityId(_entity.Id);
            drawer.Draw(_componentsContainer, _isAllOpen);
        }

        public void Destroy()
        {
            foreach (var drawer in _componentDrawers.Values)
                drawer.Destroy();
        }
    }
}
