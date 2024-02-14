using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class QuaternionDrawer : FieldDrawer<Quaternion>
    {
        private Vector3Field _field;

        protected override void Draw(string fieldName, Quaternion value, VisualElement parent, Action<Quaternion, Quaternion> onChanged)
        {
            _field = new Vector3Field(fieldName)
            {
                value = value.eulerAngles
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                var qPrev = Quaternion.Euler(ev.previousValue);
                var q = Quaternion.Euler(ev.newValue);
                onChanged?.Invoke(qPrev, q);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<Quaternion> getter)
        {
            _field.value = getter().eulerAngles;
        }
    }
}