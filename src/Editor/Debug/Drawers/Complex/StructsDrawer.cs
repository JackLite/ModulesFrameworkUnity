using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
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
        private Foldout _foldout;

        public Foldout Foldout => _foldout;

        public event Action<bool> OnChangeOpenState;

        public override bool CanDraw(object value)
        {
            if (value == null)
                return false;
            return value.GetType().IsValueType && !value.GetType().IsPrimitive;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            _foldout = new Foldout();
            _foldout.RegisterValueChangedCallback(ev => OnChangeOpenState?.Invoke(ev.newValue));
            DrawHeader(labelText, _foldout);
            foreach (var fieldInfo in value.GetType().GetFields())
            {
                // skip static fields cause anyway you can't change it
                if (fieldInfo.IsStatic)
                    continue;

                var innerFieldValue = fieldInfo.GetValue(value);

                var getter = CreateGetter(fieldInfo, value.GetType());
                var drawer = mainDrawer.Draw(fieldInfo.Name, innerFieldValue, _foldout, (_, newVal) =>
                {
                    if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    {
                        UnityEngine.Debug.LogWarning($"{fieldInfo.Name} is const. You cannot change it");
                        return;
                    }

                    fieldInfo.SetValue(value, newVal);
                    valueChangedCb(value, value);
                }, getter, Level + 1, false);

                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    drawer.SetReadOnly(true);
                _drawers.Add(drawer);
            }

            parent.Add(_foldout);
        }

        private void DrawHeader(string labelText, Foldout root)
        {
            DrawersUtil.InitObjectFieldStyle(root, Level, labelText);
            root.style.unityFontStyleAndWeight = FontStyle.Italic;
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
            foreach (var fieldDrawer in _drawers)
            {
                fieldDrawer.Update();
            }
        }

        internal void UpdateLabel(string label)
        {
            _foldout.text = label;
        }

        internal void SetOpenState(bool state)
        {
            _foldout.value = state;
        }

        internal void SetVisible(bool isVisible)
        {
            _foldout.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}