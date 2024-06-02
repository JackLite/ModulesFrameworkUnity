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
        private readonly EditorDrawer _drawer;
        private readonly StructsDrawer _structsDrawer;

        public OneDataDrawer(ModulesFramework.OneData data, VisualElement root)
        {
            _structsDrawer = new StructsDrawer();
            _drawer = new EditorDrawer();
            _structsDrawer.Init(_drawer, (o, o1) => {}, data.GetDataObject);
            var dataObject = data.GetDataObject();
            _structsDrawer.Draw(dataObject.GetType().Name, dataObject, root);
            _structsDrawer.OnChangeOpenState += OnChanged;
        }

        private void OnChanged(bool isOpened)
        {
            if(isOpened)
                DebugEventBus.Update += UpdateData;
            else
                DebugEventBus.Update -= UpdateData;
        }

        private void UpdateData()
        {
            UnityEngine.Debug.Log("test");
            _structsDrawer.Update();
        }
    }
}