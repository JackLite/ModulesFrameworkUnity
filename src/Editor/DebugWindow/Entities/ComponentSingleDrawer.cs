using System;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Draws one single component in Debug Window
    /// </summary>
    public class ComponentSingleDrawer
    {
        private readonly StructsDrawer _structsDrawer = new();
        private readonly Type _componentType;
        private readonly VisualElement _root;
        private int _eid;

        public ComponentSingleDrawer(Type componentType, VisualElement root, int eid)
        {
            _root = root;
            _componentType = componentType;
            _eid = eid;
        }

        public void SetEntityId(int eid)
        {
            _eid = eid;
        }

        public void Draw(EditorDrawer mainDrawer)
        {
            _root.Clear();
            _structsDrawer.Init(
                mainDrawer,
                (_, newValue) =>
                {
                    MF.World.GetEcsTable(_componentType).SetDataObject(_eid, newValue);
                },
                () => MF.World.GetEcsTable(_componentType).GetDataObject(_eid));

            _structsDrawer.DrawHeader(_componentType.Name);
            _structsDrawer.SetOpenState(false);
            _structsDrawer.OnChangeOpenState += OnChanged;
            _root.Add(_structsDrawer.Foldout);
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
            _root?.Clear();
        }
    }
}