using System;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one single component in Debug Window
    /// </summary>
    public class ComponentSingleDrawer : BaseComponentDrawer
    {
        private readonly StructsDrawer _structsDrawer = new();

        public ComponentSingleDrawer(Type componentType) : base(componentType)
        {
            _componentContainer.Add(_structsDrawer.Foldout);
            _structsDrawer.Foldout.SendToBack();
        }

        public void Draw(EditorDrawer mainDrawer, VisualElement root, bool isOpen)
        {
            _structsDrawer.Init(
                mainDrawer,
                (_, newValue) =>
                {
                    MF.World.GetEcsTable(_componentType).SetDataObject(_eid, newValue);
                },
                () => MF.World.GetEcsTable(_componentType).GetDataObject(_eid));

            _structsDrawer.DrawHeader(_componentType.Name);
            _structsDrawer.SetOpenState(isOpen || isAlwaysOpen);
            if (isOpen || isAlwaysOpen)
            {
                var val = MF.World.GetEcsTable(_componentType).GetDataObject(_eid);
                _structsDrawer.DrawFields(val);
                DebugEventBus.Update += Update;
            }

            _structsDrawer.OnChangeOpenState += OnOpenChanged;
            root.Add(_componentContainer);
        }

        private void OnOpenChanged(bool isOpened)
        {
            if (isOpened)
            {
                DebugEventBus.Update += Update;
                if (!_structsDrawer.IsDrawn)
                {
                    var val = MF.World.GetEcsTable(_componentType).GetDataObject(_eid);
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