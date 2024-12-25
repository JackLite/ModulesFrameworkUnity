using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public abstract class BaseCollectionDrawer<T> : FieldRefDrawer
    {
        protected Foldout _foldout;
        protected string _fieldName;
        protected readonly List<FieldDrawer> _drawers = new();
        protected T _oldRef;

        protected override void OnNullChanged()
        {
            if (_isNull)
            {
                _foldout.Clear();
                _drawers.Clear();

                if (_type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0))
                {
                    DrawCreateWidget(obj =>
                    {
                        valueChangedCb(_oldRef, obj);
                        _oldRef = (T)obj;
                        _container.Clear();
                        Draw(_fieldName, valueGetter(), _container);
                    });
                }
                else
                {
                    nullDrawer.Draw(_fieldName, typeof(void), null, _container);
                }
            }
            else
            {
                _foldout.Clear();
                Draw(_fieldName, valueGetter(), _container);
            }
        }
    }
}