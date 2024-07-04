using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one entity
    /// </summary>
    public class EntityDrawer
    {
        private readonly List<Type> _components = new();

        private Foldout _foldout;
        private EditorDrawer _mainDrawer = new();
        private Entity _entity;
        private readonly List<ComponentDrawer> _componentDrawers = new();

        public void SetEntity(Entity entity)
        {
            _entity = entity;
            _components.Clear();
            entity.World.MapTables((compType, table) =>
            {
                if (table.Contains(entity.Id))
                    _components.Add(compType);
            });
        }

        public void Draw(VisualElement root, bool isOpened)
        {
            _foldout = new Foldout();
            _foldout.AddToClassList("modules--one-entity");
            root.Add(_foldout);

            var name = _components.Select(t => t.Name).OrderBy(n => n).Aggregate((a, b) => a + ", " + b);
            _foldout.text = name;

            foreach (var componentType in _components)
            {
                var componentDrawer = new ComponentDrawer();
                componentDrawer.Draw(_mainDrawer, _entity.Id, componentType, _foldout);
            }
        }
    }
}