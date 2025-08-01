using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Complex
{
    public class StructsDrawer : FieldDrawer
    {
        public override int Order => 100;
        private readonly List<FieldDrawer> _drawers = new();

        public Foldout Foldout { get; } = new();
        public bool IsDrawn { get; protected set; }

        public event Action<bool> OnChangeOpenState;

        public StructsDrawer()
        {
            Foldout.AddToClassList("modules--struct-drawer--foldout");
        }

        public override void Init(EditorDrawer drawer, Action<object, object> onChanged, Func<object> getter)
        {
            base.Init(drawer, onChanged, getter);
            Foldout.RegisterValueChangedCallback(ev =>
            {
                if (ev.target == Foldout)
                    OnChangeOpenState?.Invoke(ev.newValue);
            });
        }

        public override bool CanDraw(Type type, object value)
        {
            if (value == null)
                return false;
            return type.IsValueType && !type.IsPrimitive;
        }

        protected override void Draw(string labelText, object value, VisualElement parent)
        {
            DrawHeader(labelText);
            DrawFields(value);

            parent.Add(Foldout);
        }

        public void DrawFields(object value)
        {
            IsDrawn = true;
            foreach (var fieldInfo in value.GetType().GetFields())
            {
                // skip static fields cause anyway you can't change it
                if (fieldInfo.IsStatic)
                    continue;

                var innerFieldValue = fieldInfo.GetValue(value);

                var getter = CreateGetter(fieldInfo, value.GetType());
                var drawer = mainDrawer.Draw(fieldInfo.Name, fieldInfo.FieldType, innerFieldValue, Foldout,
                    (_, newVal) =>
                    {
                        var realValue = valueGetter();
                        if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                        {
                            UnityEngine.Debug.LogWarning($"{fieldInfo.Name} is const. You cannot change it");
                            return;
                        }

                        fieldInfo.SetValue(realValue, newVal);
                        valueChangedCb(realValue, realValue);
                    }, getter, Level + 1, false);

                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    drawer.SetReadOnly(true);
                _drawers.Add(drawer);
            }
        }

        public void DrawHeader(string labelText)
        {
            DrawersUtil.InitObjectFieldStyle(Foldout, Level, labelText);
        }

        private Func<object> CreateGetter(FieldInfo field, Type structType)
        {
            if (field.IsLiteral && !field.IsInitOnly)
            {
                var constData = Expression.Constant(field.GetValue(null));
                return Expression.Lambda<Func<object>>(Expression.Convert(constData, typeof(object))).Compile();
            }

            Expression<Func<object>> getExpr = () => valueGetter();
            var callData = Expression.Invoke(getExpr);
            var converted = Expression.Convert(callData, structType);
            var fieldData = Expression.Field(converted, field.Name);
            return Expression.Lambda<Func<object>>(Expression.Convert(fieldData, typeof(object))).Compile();
        }

        public override void Update()
        {
            if (Foldout.value == false)
                return;
            foreach (var fieldDrawer in _drawers)
            {
                fieldDrawer.Update();
            }
        }

        internal void UpdateLabel(string label)
        {
            Foldout.text = label;
        }

        internal void SetOpenState(bool state)
        {
            Foldout.value = state;
        }

        internal void SetVisible(bool isVisible)
        {
            Foldout.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void Reset()
        {
            _drawers.Clear();
            Foldout.Clear();
            IsDrawn = false;
            OnChangeOpenState = null;
        }

        public void SetGetter(Func<object> getter)
        {
            valueGetter = getter;
        }
    }
}
