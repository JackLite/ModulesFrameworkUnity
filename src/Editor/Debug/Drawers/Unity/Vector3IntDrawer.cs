﻿using System;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class Vector3IntDrawer : FieldDrawer<Vector3Int>
    {
        private Vector3IntField _field;

        protected override void Draw(string labelText, Vector3Int value, VisualElement parent, Action<Vector3Int, Vector3Int> onChanged)
        {
            _field = new Vector3IntField(labelText)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<Vector3Int> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }
    }
}