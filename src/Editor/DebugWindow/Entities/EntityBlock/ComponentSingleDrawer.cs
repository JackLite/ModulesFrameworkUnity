using System;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
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
            _pinButton.BringToFront();
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
            _structsDrawer.SetOpenState(isOpen);
            if (isOpen)
            {
                var val = MF.World.GetEcsTable(_componentType).GetDataObject(_eid);
                _structsDrawer.DrawFields(val);
                DebugEventBus.Update += Update;
            }

            _structsDrawer.OnChangeOpenState += OnChanged;
            root.Add(_componentContainer);
        }

        private void OnChanged(bool isOpened)
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
    }
}