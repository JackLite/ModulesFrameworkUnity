using System;
using System.Globalization;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneData
{
    /// <summary>
    ///     Draws OneData inside debug window
    /// </summary>
    public class OneDataDrawer
    {
        private readonly StructsDrawer _structsDrawer;

        public Type DataType { get; private set; }

        public OneDataDrawer(ModulesFramework.OneData data, VisualElement root)
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
            _structsDrawer.SetOpenState(false);
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

        public void SetVisible(bool isMatch)
        {
            _structsDrawer.SetVisible(isMatch);
        }
    }
}