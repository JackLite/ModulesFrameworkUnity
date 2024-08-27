using System;
using ModulesFrameworkUnity.Debug.Drawers.Special;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers
{
    public abstract class FieldDrawer
    {
        protected EditorDrawer mainDrawer;
        protected Func<object> valueGetter;
        protected Action<object, object> valueChangedCb;
        protected NullDrawer nullDrawer;

        public virtual int Order => 0;
        public int Level { get; set; }

        public virtual void Init(EditorDrawer drawer, Action<object, object> onChanged, Func<object> getter)
        {
            mainDrawer = drawer;
            valueChangedCb = onChanged;
            valueGetter = getter;

            nullDrawer = new()
            {
                mainDrawer = drawer,
                valueChangedCb = onChanged,
                valueGetter = getter
            };
        }

        public abstract bool CanDraw(object value);

        public abstract void Draw(string labelText, object value, VisualElement parent);

        public abstract void Update();

        public virtual void SetReadOnly(bool isReadOnly)
        {
        }
    }

    /// <summary>
    ///     Base class for draw the reference types
    /// </summary>
    public abstract class FieldRefDrawer : FieldDrawer
    {
        protected bool _isNull;
        protected VisualElement _container;

        protected void ProceedNull()
        {
            var value = valueGetter();
            if (value == null && !_isNull)
            {
                mainDrawer.RemoveDrawer(this);
                _container.Clear();
                _isNull = true;
                OnNullChanged();
            }
            else if (value != null && _isNull)
            {
                mainDrawer.RemoveDrawer(nullDrawer);
                _container.Clear();
                _isNull = false;
                OnNullChanged();
            }
        }

        /// <summary>
        ///     Calls after value was set to null or vice versa
        /// </summary>
        protected virtual void OnNullChanged()
        {
        }
    }

    public abstract class FieldDrawer<T> : FieldDrawer
    {
        protected abstract void Draw(string labelText, T value, VisualElement parent, Action<T, T> onChanged);

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            Draw(labelText, (T)value, parent, (prev, newVal) => valueChangedCb?.Invoke(prev, newVal));
        }

        public override bool CanDraw(object value)
        {
            return value is T;
        }

        public override void Update()
        {
            if (valueGetter() == null)
                return;

            Update(() => (T)valueGetter());
        }

        protected abstract void Update(Func<T> getter);
    }
}