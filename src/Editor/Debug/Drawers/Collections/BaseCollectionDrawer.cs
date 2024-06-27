using System.Collections.Generic;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public abstract class BaseCollectionDrawer : FieldRefDrawer
    {
        protected Foldout _foldout;
        protected string _fieldName;
        protected readonly List<FieldDrawer> _drawers = new();
        protected override void OnNullChanged()
        {
            if (_isNull)
            {
                _foldout.Clear();
                _drawers.Clear();
                nullDrawer.Draw(_fieldName, null, _container);
            }
            else
            {
                _foldout.Clear();
                Draw(_fieldName, valueGetter(), _container);
            }
        }
    }
}