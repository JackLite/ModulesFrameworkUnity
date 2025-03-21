using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using System;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one single component in Debug Window
    /// </summary>
    public class ComponentSingleDrawer : BaseComponentDrawer
    {
        private readonly StructsDrawer _structsDrawer = new();
        private ContextualMenuManipulator _contextMenuManipulator;

        public ComponentSingleDrawer(Type componentType) : base(componentType)
        {
            _componentContainer.Add(_structsDrawer.Foldout);
            _structsDrawer.Foldout.SendToBack();
            InitContextMenu();
        }

        public void Draw(EditorDrawer mainDrawer, VisualElement root, bool isOpen)
        {
            _structsDrawer.Init(
                mainDrawer,
                (_, newValue) =>
                {
                    DebugUtils.GetCurrentWorld().GetEcsTable(_componentType).SetDataObject(_eid, newValue);
                },
                () => DebugUtils.GetCurrentWorld().GetEcsTable(_componentType).GetDataObject(_eid));

            _structsDrawer.DrawHeader(_componentType.Name);
            _structsDrawer.SetOpenState(isOpen || isAlwaysOpen);
            if (isOpen || isAlwaysOpen)
            {
                var val = DebugUtils.GetCurrentWorld().GetEcsTable(_componentType).GetDataObject(_eid);
                _structsDrawer.DrawFields(val);
                DebugEventBus.Update += Update;
            }

            _structsDrawer.OnChangeOpenState += OnOpenChanged;
            _structsDrawer.Foldout.Q(className: Foldout.toggleUssClassName).Add(_componentControlPanel);
            root.Add(_componentContainer);
        }

        private void InitContextMenu()
        {
            _contextMenuManipulator = new ContextualMenuManipulator(BuildContextMenu);
            var componentTitle = _structsDrawer.Foldout.Q(className: Foldout.toggleUssClassName);
            componentTitle.AddManipulator(_contextMenuManipulator);
        }

        private void BuildContextMenu(ContextualMenuPopulateEvent ev)
        {
            ev.menu.AppendAction("Remove", a =>
            {
                var table = DebugUtils.GetCurrentWorld().GetEcsTable(_componentType);
                table.Remove(_eid);
            });
        }

        private void OnOpenChanged(bool isOpened)
        {
            if (isOpened)
            {
                DebugEventBus.Update += Update;
                if (!_structsDrawer.IsDrawn)
                {
                    var val = DebugUtils.GetCurrentWorld().GetEcsTable(_componentType).GetDataObject(_eid);
                    _structsDrawer.DrawFields(val);
                }
            }
            else
            {
                DebugEventBus.Update -= Update;
            }
        }

        private void Update()
        {
            _structsDrawer.Update();
        }

        public void Destroy()
        {
            DebugEventBus.Update -= Update;
            _structsDrawer.Reset();
            _componentContainer?.RemoveFromHierarchy();
        }

        protected override void OnAlwaysOpenChanged()
        {
            _structsDrawer.SetOpenState(isAlwaysOpen);
        }
    }
}
