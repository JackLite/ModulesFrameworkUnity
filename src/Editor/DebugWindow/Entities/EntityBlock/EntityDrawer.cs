using Modules.Extensions.Prototypes.Editor.AddingComponents;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug.Entities.EntityBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFrameworkUnity.DebugWindow.Entities.AddComponent;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one entity
    /// </summary>
    public class EntityDrawer : ScrollView
    {
        private Entity _entity;

        private EntityButtons _entityButtons;
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
            if (_entityButtons == null)
            {
                _entityButtons = new EntityButtons();
                _entityButtons.Draw();
                _entityButtons.OnAddComponentClick += OnAddComponentClick;
                _entityButtons.OnDestroyClick += OnDestroyClick;
                Add(_entityButtons);
            }

            if (_singleComponents == null)
            {
                _singleComponents = new EntitySingleComponents(pinnedComponents);
                _singleComponents.SetEntity(_entity);
                _singleComponents.OnPinComponent += OnPin;
                Add(_singleComponents);
            }

            if (_multipleComponents == null)
            {
                _multipleComponents = new EntityMultipleComponents(pinnedComponents);
                _multipleComponents.SetEntity(_entity);
                _multipleComponents.OnPinComponent += OnPin;
                Add(_multipleComponents);
            }

            _entityButtons.style.display = DisplayStyle.Flex;
            _singleComponents.Draw(isAllOpen);
            _multipleComponents.Draw(isAllOpen);

            MF.World.OnEntityChanged += OnEntityChanged;
        }

        private void OnDestroyClick()
        {
            _entity.Destroy();
            Destroy();
        }

        private void OnAddComponentClick()
        {
            var window = ScriptableObject.CreateInstance<AddComponentWindow>();
            window.Show(_entity.Id);
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
            if (_entityButtons != null)
                _entityButtons.style.display = DisplayStyle.None;
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