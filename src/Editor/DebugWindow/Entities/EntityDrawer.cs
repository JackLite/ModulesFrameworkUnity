using System;
using System.Collections.Generic;
using System.Text;
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

        private EntitySingleComponents _singleComponents;
        private EntityMultipleComponents _multipleComponents;

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

        public void Draw()
        {
            if (_singleComponents == null)
            {
                _singleComponents ??= new EntitySingleComponents();
                _singleComponents.SetEntity(_entity);
                Add(_singleComponents);
            }

            if(_multipleComponents == null)
            {
                _multipleComponents ??= new EntityMultipleComponents();
                _multipleComponents.SetEntity(_entity);
                Add(_multipleComponents);
            }

            _singleComponents.Draw();
            _multipleComponents.Draw();

            MF.World.OnEntityChanged += OnEntityChanged;
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
        }
    }
}