using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one entity
    /// </summary>
    public class EntityDrawer : ScrollView
    {
        private Entity _entity;

        private EntityDrawerSettings _settings;
        private EntitySingleComponents _singleComponents;
        private EntityMultipleComponents _multipleComponents;

        public event Action<HashSet<string>> OnPinComponent;

        public EntityDrawer()
        {
            AddToClassList("modules--one-entity");
        }

        public void SetEntity(Entity entity)
        {
            _entity = entity;
            _singleComponents?.SetEntity(_entity);
            _multipleComponents?.SetEntity(_entity);
        }

        public void Draw(bool isAllOpen, IEnumerable<string> pinnedComponents)
        {
            if (_singleComponents == null)
            {
                _singleComponents = new EntitySingleComponents(pinnedComponents);
                _singleComponents.SetEntity(_entity);
                _singleComponents.OnPinComponent += OnPin;
                Add(_singleComponents);
            }

            if(_multipleComponents == null)
            {
                _multipleComponents = new EntityMultipleComponents(pinnedComponents);
                _multipleComponents.SetEntity(_entity);
                _multipleComponents.OnPinComponent += OnPin;
                Add(_multipleComponents);
            }

            _singleComponents.Draw(isAllOpen);
            _multipleComponents.Draw(isAllOpen);

            MF.World.OnEntityChanged += OnEntityChanged;
        }

        private void OnPin()
        {
            var allPinned = _singleComponents.pinnedComponents
                .Union(_multipleComponents.pinnedComponents)
                .ToHashSet();
            OnPinComponent?.Invoke(allPinned);
        }

        public void Destroy()
        {
            _singleComponents?.Destroy();
            _multipleComponents?.Destroy();
            MF.World.OnEntityChanged -= OnEntityChanged;
        }

        private void OnEntityChanged(int eid)
        {
            if (_entity.Id != eid)
                return;

            _singleComponents?.OnEntityChanged();
            _multipleComponents?.OnEntityChanged();
        }
    }
}