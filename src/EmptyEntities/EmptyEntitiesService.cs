using System.Collections.Generic;
using ModulesFramework.Data;

namespace ModulesFrameworkUnity.EmptyEntities
{
    public class EmptyEntitiesService
    {
        private readonly DataWorld _world;
        private readonly HashSet<int> _emptyEntities = new();

        public EmptyEntitiesService(DataWorld world)
        {
            _world = world;
            _world.OnEntityCreated += OnEntityCreated;
            _world.OnEntityChanged += OnEntityChanged;
        }

        public void RemoveEmpty()
        {
            if(_emptyEntities.Count == 0)
                return;

            foreach (var eid in _emptyEntities)
            {
                if (_world.IsEntityAlive(eid))
                    _world.DestroyEntity(eid);
            }

            _emptyEntities.Clear();
        }

        private void OnEntityCreated(int eid)
        {
            _emptyEntities.Add(eid);
        }

        private void OnEntityChanged(int eid)
        {
            if (_world.IsEmptyEntity(eid) && !_emptyEntities.Contains(eid))
                _emptyEntities.Add(eid);
            else if (!_world.IsEmptyEntity(eid) && _emptyEntities.Contains(eid))
                _emptyEntities.Remove(eid);
        }
    }
}