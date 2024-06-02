using System;
using ModulesFramework;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneData
{
    /// <summary>
    ///     List of one data-s
    /// </summary>
    public class OneDataTab : ScrollView
    {
        private readonly EditorDrawer _drawer;

        public OneDataTab()
        {
            _drawer = new EditorDrawer();
            MF.World.OnOneDataCreated += OnCreated;
            foreach (var data in MF.World.OneDataCollection)
            {
                OnCreated(data.GetDataObject().GetType(), data);
            }
        }

        private void OnCreated(Type dataType, ModulesFramework.OneData data)
        {
            var dataDrawer = new OneDataDrawer(data, this);
        }
    }
}