using ModulesFramework;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using System;
using System.Globalization;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    /// <summary>
    ///     Draws OneData inside debug window
    /// </summary>
    public class OneDataDrawer
    {
        private readonly StructsDrawer _structsDrawer;

        public OneDataDrawer(Type dataType, VisualElement root)
        {
            var oneData = DebugUtils.GetCurrentWorld().GetOneDataWrapper(dataType);
            if (oneData == null)
            {
                UnityEngine.Debug.LogError($"[Modules.Debug] OneData {dataType.GetTypeName()} not found");
                return;
            }

            _structsDrawer = new StructsDrawer();
            var drawer = new EditorDrawer();
            var dataObject = oneData.GetDataObject();
            var typeName = dataObject.GetType().GetTypeName();
            _structsDrawer.Init(drawer, (_, newVal) =>
            {
                var dataWrapper = DebugUtils.GetCurrentWorld().GetOneDataWrapper(dataType);
                if (dataWrapper == null)
                    return;
                var gen = $" [Gen {dataWrapper.generation.ToString(CultureInfo.InvariantCulture)}]";
                dataWrapper.SetDataObject(newVal);
                _structsDrawer.UpdateLabel(typeName + gen);
            }, oneData.GetDataObject);
            var gen = $" [Gen {oneData.generation.ToString(CultureInfo.InvariantCulture)}]";
            _structsDrawer.Draw(typeName + gen, dataType, dataObject, root);
            _structsDrawer.SetOpenState(true);
            _structsDrawer.OnChangeOpenState += OnChanged;
        }

        private void OnChanged(bool isOpened)
        {
            if (isOpened)
                DebugEventBus.Update += UpdateData;
            else
                DebugEventBus.Update -= UpdateData;
        }

        private void UpdateData()
        {
            _structsDrawer.Update();
        }

        public void SetVisible(bool isVisible)
        {
            _structsDrawer.SetVisible(isVisible);
            if (isVisible)
                DebugEventBus.Update += UpdateData;
            else
                DebugEventBus.Update -= UpdateData;
        }

        public void Destroy()
        {
            _structsDrawer.Foldout.style.display = DisplayStyle.None;
            DebugEventBus.Update -= UpdateData;
        }

        public void UpdateWrapper(Type dataType, VisualElement root)
        {
            var oneData = DebugUtils.GetCurrentWorld().GetOneDataWrapper(dataType);
            if (oneData == null)
            {
                UnityEngine.Debug.LogError($"[Modules.Debug] OneData {dataType.GetTypeName()} not found");
                return;
            }

            _structsDrawer.Reset();
            _structsDrawer.SetGetter(oneData.GetDataObject);
            var dataObject = oneData.GetDataObject();
            var typeName = dataObject.GetType().Name;
            var gen = $" [Gen {oneData.generation.ToString(CultureInfo.InvariantCulture)}]";
            _structsDrawer.Draw(typeName + gen, dataType, dataObject, root);
        }
    }
}
