﻿using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    public abstract class FieldDrawer
    {
        protected EditorDrawer mainDrawer;
        protected Func<object> valueGetter;
        protected Action<object, object> valueChangedCb;

        public virtual int Order => 0;

        public void Init(EditorDrawer drawer, Action<object, object> onChanged, Func<object> getter)
        {
            mainDrawer = drawer;
            valueChangedCb = onChanged;
            valueGetter = getter;
        }

        public abstract bool CanDraw(object value);

        public abstract void Draw(string fieldName, object value, VisualElement parent);

        public abstract void Update();
        public virtual void SetReadOnly(bool isReadOnly)
        {
        }
    }

    public abstract class FieldDrawer<T> : FieldDrawer
    {
        protected abstract void Draw(string fieldName, T value, VisualElement parent, Action<T, T> onChanged);

        public override void Draw(string fieldName, object value, VisualElement parent)
        {
            Draw(fieldName, (T)value, parent, (prev, newVal) => valueChangedCb?.Invoke(prev, newVal));
        }

        public override bool CanDraw(object value)
        {
            return value is T;
        }

        public override void Update()
        {
            Update(() => (T)valueGetter());
        }

        protected abstract void Update(Func<T> getter);
    }
}