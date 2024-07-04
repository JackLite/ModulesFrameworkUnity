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
    public class AnyClassDrawer : FieldRefDrawer
    {
        public override int Order => 10000;
        private readonly List<FieldDrawer> _drawers = new();
        private Foldout _foldout;
        private string _labelText;

        public override bool CanDraw(object value)
        {
            if (value == null)
                return false;
            var type = value.GetType();
            return !type.IsValueType
                   && !type.IsPrimitive;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            _container = new();
            _foldout = new Foldout();
            _container.Add(_foldout);
            _labelText = labelText;
            DrawInternal(value);
            parent.Add(_container);
        }

        private void DrawInternal(object value)
        {
            DrawHeader(_labelText, _foldout);
            foreach (var fieldInfo in value.GetType().GetFields())
            {
                var innerFieldValue = fieldInfo.GetValue(value);

                var getter = CreateGetter(fieldInfo, value.GetType());
                var drawer = mainDrawer.Draw(fieldInfo.Name, innerFieldValue, _foldout, (prev, newVal) =>
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
        }

        private void DrawHeader(string labelText, Foldout root)
        {
            DrawersUtil.InitObjectFieldStyle(root, Level, labelText + valueGetter().GetHashCode());
        }

        private Func<object> CreateGetter(FieldInfo field, Type structType)
        {
            if (field.IsLiteral && !field.IsInitOnly)
            {
                var constData = Expression.Constant(field.GetValue(null));
                return Expression.Lambda<Func<object>>(Expression.Convert(constData, typeof(object))).Compile();
            }

            if (field.IsStatic)
            {
                var staticField = Expression.Field(null, field);
                return Expression.Lambda<Func<object>>(Expression.Convert(staticField, typeof(object))).Compile();
            }

            Expression<Func<object>> getExpr = () => valueGetter();
            var callData = Expression.Invoke(getExpr);
            var converted = Expression.Convert(callData, structType);
            var fieldData = Expression.Field(converted, field.Name);
            return Expression.Lambda<Func<object>>(Expression.Convert(fieldData, typeof(object))).Compile();
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;

            foreach (var fieldDrawer in _drawers)
            {
                fieldDrawer.Update();
            }
        }

        protected override void OnNullChanged()
        {
            if (_isNull)
            {
                _foldout.Clear();
                _drawers.Clear();
                nullDrawer.Draw(_labelText, null, _container);
            }
            else
            {
                _foldout.Clear();
                Draw(_labelText, valueGetter(), _container);
            }
        }
    }
}