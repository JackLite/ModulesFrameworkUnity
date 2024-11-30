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
    public class MultipleComponentDrawer : BaseComponentDrawer
    {
        private Foldout _foldout;
        private readonly Stack<StructDrawerDecorator> _drawersPool = new();
        private readonly List<StructDrawerDecorator> _decorators = new();
        private readonly Dictionary<int, object> _values = new();
        private readonly EditorDrawer _mainDrawer;

        public MultipleComponentDrawer(Type componentType, EditorDrawer mainDrawer) : base(componentType)
        {
            _mainDrawer = mainDrawer;
        }

        public void Draw(VisualElement root, bool isOpened)
        {
            if (_foldout == null)
            {
                _foldout = new Foldout();
                _foldout.RegisterValueChangedCallback(ev =>
                {
                    if (ev.target != _foldout)
                        return;
                    OnFoldoutChanged(ev.newValue);
                });
                _componentContainer.Add(_foldout);
                _foldout.SendToBack();
            }

            var table = MF.World.GetEcsTable(_componentType);
            var count = table.GetMultipleDataLength(_eid);
            _foldout.text = $"{_componentType.Name} ({count})";
            _foldout.value = isOpened || isAlwaysOpen;

            root.Add(_componentContainer);

            if (isOpened || isAlwaysOpen)
            {
                DrawComponents();
                DebugEventBus.Update += Update;
            }
        }

        private void OnFoldoutChanged(bool isOpened)
        {
            if (isOpened)
            {
                Reset();
                DrawComponents();
                DebugEventBus.Update += Update;
            }
            else
            {
                Reset();
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
                if (!_drawersPool.TryPop(out var decorator))
                    decorator = new StructDrawerDecorator();

                decorator.Init(_eid, _componentType, index);
                decorator.drawer.Init(_mainDrawer, (_, newValue) =>
                    {
                        _values[index] = newValue;
                        table.SetDataObjects(_eid, _values.Values.ToList());
                    },
                    () => table.GetAt(index));
                decorator.drawer.Draw($"{component.GetType().Name}", component, _foldout);
                _decorators.Add(decorator);
            }
        }

        private void Update()
        {
            foreach (var decorator in _decorators)
                decorator.drawer.Update();
        }

        public void OnEntityChanged()
        {
            var table = MF.World.GetEcsTable(_componentType);
            var currentCount = table.GetMultipleDataLength(_eid);
            if (_decorators.Count == currentCount)
                return;

            Reset();
            _foldout.text = $"{_componentType.Name} ({currentCount})";
            DrawComponents();
        }

        private void Reset()
        {
            foreach (var decorator in _decorators)
            {
                decorator.drawer.Reset();
                _drawersPool.Push(decorator);
            }

            _decorators.Clear();
            _foldout?.Clear();
        }

        public void Destroy()
        {
            Reset();
            _componentContainer?.RemoveFromHierarchy();
        }

        protected override void OnAlwaysOpenChanged()
        {
            _foldout.value = isAlwaysOpen;
        }

        private class StructDrawerDecorator
        {
            public StructsDrawer drawer = new();

            private int _index;
            private readonly ContextualMenuManipulator _contextualMenuManipulator;
            private Type _componentType;
            private int _eid;

            public StructDrawerDecorator()
            {
                _contextualMenuManipulator = new ContextualMenuManipulator(BuildContextMenu);
                drawer.Foldout.Q(className: Foldout.toggleUssClassName).AddManipulator(_contextualMenuManipulator);
            }

            public void Init(int eid, Type componentType, int index)
            {
                _index = index;
                _componentType = componentType;
                _eid = eid;
            }

            private void BuildContextMenu(ContextualMenuPopulateEvent ev)
            {
                ev.menu.AppendAction("Remove", _ =>
                {
                    var table = MF.World.GetEcsTable(_componentType);
                    table.RemoveByDenseIndex(_eid, _index);
                });
            }
        }
    }
}