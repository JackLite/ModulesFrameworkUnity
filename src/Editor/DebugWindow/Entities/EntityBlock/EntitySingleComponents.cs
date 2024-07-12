using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Controls of view for single components
    /// </summary>
    public class EntitySingleComponents : VisualElement
    {
        private readonly Dictionary<Type, ComponentSingleDrawer> _componentDrawers = new();
        private Entity _entity;
        private readonly HashSet<Type> _components = new();
        private readonly HashSet<Type> _old = new();
        private readonly HashSet<Type> _new = new();
        private readonly EditorDrawer _mainDrawer = new();
        private readonly VisualElement _componentsContainer;
        private bool _isOpenAll;

        public EntitySingleComponents()
        {
            var title = new Label("Single components");
            title.AddToClassList("modules--one-entity--components-title");
            Add(title);
            _componentsContainer = new();
            Add(_componentsContainer);
        }

        public void SetEntity(Entity entity)
        {
            _entity = entity;
            _components.Clear();
            foreach (var componentType in MF.World.GetEntitySingleComponentsType(_entity.Id))
                _components.Add(componentType);
        }

        public void Draw(bool isOpenAll)
        {
            _isOpenAll = isOpenAll;
            _componentsContainer.Clear();
            foreach (var componentType in _components)
                DrawComponent(componentType);
            Reorder();
        }

        public void OnEntityChanged()
        {
            if (!_entity.IsAlive())
                return;
            _old.Clear();
            _new.Clear();
            _old.UnionWith(_components);
            foreach (var componentType in MF.World.GetEntitySingleComponentsType(_entity.Id))
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
                DrawComponent(componentType);
                _components.Add(componentType);
            }

            Reorder();
        }

        private void Reorder()
        {
            foreach (var (_, drawer) in _componentDrawers.OrderByDescending(kvp => kvp.Key.Name))
                drawer.SetFirst();
        }

        private void DrawComponent(Type componentType)
        {
            if (!_componentDrawers.TryGetValue(componentType, out var drawer))
            {
                drawer = new ComponentSingleDrawer(componentType, _entity.Id);
                _componentDrawers[componentType] = drawer;
            }

            drawer.Destroy();
            drawer.SetEntityId(_entity.Id);
            drawer.Draw(_mainDrawer, _componentsContainer, _isOpenAll);
        }

        public void Destroy()
        {
            foreach (var componentDrawer in _componentDrawers.Values)
                componentDrawer.Destroy();
            _componentsContainer.Clear();
        }
    }
}