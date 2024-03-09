using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Complex
{
    public class AnyClassDrawer : FieldDrawer
    {
        public override int Order => 10000;
        private readonly List<FieldDrawer> _drawers = new();

        public override bool CanDraw(object value)
        {
            if (value == null)
                return false;
            var type = value.GetType();
            return !type.IsValueType
                   && !type.IsPrimitive
                   && type.GetConstructor(Type.EmptyTypes) != null;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            var root = new VisualElement();
            DrawHeader(labelText, root);
            foreach (var fieldInfo in value.GetType().GetFields())
            {
                var innerFieldValue = fieldInfo.GetValue(value);

                var getter = CreateGetter(fieldInfo, value.GetType());
                var drawer = mainDrawer.Draw(fieldInfo.Name, innerFieldValue, root, (prev, newVal) =>
                {
                    if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    {
                        UnityEngine.Debug.LogWarning($"{fieldInfo.Name} is const. You cannot change it");
                        return;
                    }

                    fieldInfo.SetValue(value, newVal);
                    valueChangedCb(value, value);
                }, getter, false);

                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    drawer.SetReadOnly(true);
                _drawers.Add(drawer);
            }

            parent.Add(root);
        }
        
        private void DrawHeader(string labelText, VisualElement root)
        {
            var label = new Label(labelText)
            {
                style =
                {
                    fontSize = mainDrawer.Level > 1 ? 12 : 14,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.BoldAndItalic),
                    marginTop = new StyleLength(8),
                    marginBottom = new StyleLength(8)
                }
            };
            root.Add(label);
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
            foreach (var fieldDrawer in _drawers)
            {
                fieldDrawer.Update();
            }
        }
    }
}