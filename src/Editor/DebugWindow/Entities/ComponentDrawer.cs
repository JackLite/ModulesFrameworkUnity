using System;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one component in Debug Window
    /// </summary>
    public class ComponentDrawer
    {
        private StructsDrawer _structsDrawer;

        public void Draw(EditorDrawer mainDrawer, int eid, Type componentType, VisualElement root)
        {
            _structsDrawer = new StructsDrawer();
            var val = MF.World.GetEcsTable(componentType).GetDataObject(eid);
            _structsDrawer.Init(mainDrawer, (_, newValue) =>
            {
                MF.World.GetEcsTable(componentType).SetDataObject(eid, newValue);
            }, () =>
            {
                return MF.World.GetEcsTable(componentType).GetDataObject(eid);
            });
            _structsDrawer.Draw(componentType.Name, val, root);
            _structsDrawer.SetOpenState(true);
            _structsDrawer.OnChangeOpenState += OnChanged;
            DebugEventBus.Update += Update;
        }

        private void OnChanged(bool isOpened)
        {
            if (isOpened)
                DebugEventBus.Update += Update;
            else
                DebugEventBus.Update -= Update;
        }

        private void Update()
        {
            _structsDrawer.Update();
        }
    }
}