using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws components that marked as multiple
    /// </summary>
    public class MultipleComponentDrawer
    {
        private readonly Type _componentType;
        private Foldout _foldout;
        private int _eid;
        private readonly Stack<StructsDrawer> _drawersPool = new();
        private readonly List<StructsDrawer> _drawers = new();
        private readonly Dictionary<int, object> _values = new();
        private readonly EditorDrawer _mainDrawer;

        public MultipleComponentDrawer(Type componentType, EditorDrawer mainDrawer)
        {
            _componentType = componentType;
            _mainDrawer = mainDrawer;
        }

        public void SetEntityId(int eid)
        {
            _eid = eid;
        }

        public void Draw(VisualElement root, bool isOpened)
        {
            if (_foldout == null)
            {
                _foldout = new Foldout();
                _foldout.RegisterValueChangedCallback(ev => OnFoldoutChanged(ev.newValue));
            }

            var table = MF.World.GetEcsTable(_componentType);
            var count = table.Count(_eid);
            _foldout.text = $"{_componentType.Name} ({count})";
            _foldout.value = isOpened;

            root.Add(_foldout);

            if (isOpened)
            {
                DrawComponents();
                DebugEventBus.Update += Update;
            }
        }

        private void OnFoldoutChanged(bool isOpened)
        {
            if (isOpened)
            {
                Destroy();
                DrawComponents();
                DebugEventBus.Update += Update;
            }
            else
            {
                DebugEventBus.Update -= Update;
            }
        }

        private void DrawComponents()
        {
            _values.Clear();
            var table = MF.World.GetEcsTable(_componentType);
            table.GetDataObjects(_eid, _values);

            foreach (var (index, component) in _values)
            {
                if (!_drawersPool.TryPop(out var drawer))
                    drawer = new StructsDrawer();
                drawer.Init(_mainDrawer, (_, newValue) =>
                    {
                        _values[index] = newValue;
                        table.SetDataObjects(_eid, _values.Values.ToList());
                    },
                    () => table.GetAt(index));
                drawer.Draw($"{component.GetType().Name} [{index}]", component, _foldout);
                drawer.SetOpenState(true);
                _drawers.Add(drawer);
            }
        }

        private void Update()
        {
            foreach (var drawer in _drawers)
                drawer.Update();
        }

        public void Destroy()
        {
            foreach (var drawer in _drawers)
            {
                drawer.Reset();
                _drawersPool.Push(drawer);
            }

            _drawers.Clear();
        }
    }
}