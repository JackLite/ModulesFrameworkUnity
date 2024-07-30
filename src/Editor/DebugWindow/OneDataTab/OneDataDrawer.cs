using System;
using System.Globalization;
using ModulesFramework;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    /// <summary>
    ///     Draws OneData inside debug window
    /// </summary>
    public class OneDataDrawer
    {
        private readonly StructsDrawer _structsDrawer;
        private Button _pinBtn;

        public Type DataType { get; }

        public OneDataDrawer(OneData data, VisualElement root)
        {
            _structsDrawer = new StructsDrawer();
            var drawer = new EditorDrawer();
            var dataObject = data.GetDataObject();
            DataType = dataObject.GetType();
            var typeName = DataType.Name;
            _structsDrawer.Init(drawer, (_, newVal) =>
            {
                var gen = $" [Gen {data.generation.ToString(CultureInfo.InvariantCulture)}]";
                data.SetDataObject(newVal);
                _structsDrawer.UpdateLabel(typeName + gen);
            }, data.GetDataObject);
            var gen = $" [Gen {data.generation.ToString(CultureInfo.InvariantCulture)}]";
            _structsDrawer.Draw(typeName + gen, dataObject, root);
            _structsDrawer.SetOpenState(true);
            DebugEventBus.Update += UpdateData;
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
    }
}