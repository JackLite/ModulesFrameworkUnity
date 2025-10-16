using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFrameworkUnity.Debug.Drawers.Collections;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.UpdateDebuggingEditor.Service
{
    public class QueueModulesDrawer : BaseCollectionDrawer<Queue<ModuleDebugWrapper>>
    {
        private VisualElement _elementsParent;
        public override bool CanDraw(Type type, object value)
        {
            return value is Queue<ModuleDebugWrapper>;
        }

        protected override void Draw(string labelText, object value, VisualElement parent)
        {
            _fieldName = labelText;
            _container = new VisualElement();
            parent.Add(_container);
            _foldout = new Foldout
            {
                text = $"{labelText} [0]",
                value = false,
            };

            _elementsParent = new VisualElement();
            _foldout.Add(_elementsParent);
            _container.Add(_foldout);

            if (value == null)
                return;

            _oldRef = (Queue<ModuleDebugWrapper>)value;

            _foldout.text = $"{labelText} [{_oldRef.Count}]";

            DrawQueue(value);

        }

        private void DrawQueue(object value)
        {
            _drawers.Clear();
            var queue =  value as Queue<ModuleDebugWrapper>;
            foreach (var wrapper in queue)
            {
                var elementContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
                };
                var wrapper1 = wrapper;
                var drawer = mainDrawer.Draw(wrapper.module.GetType().Name, wrapper.GetType(), wrapper, _elementsParent,
                    (old, newValue) => { }, () => wrapper1, Level + 1, false);
                _drawers.Add(drawer);
                _elementsParent.Add(elementContainer);
            }
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;
            var queue = (Queue<ModuleDebugWrapper>)valueGetter();
            _foldout.text = $"{_fieldName} [{_oldRef.Count}]";

            if (!ReferenceEquals(_oldRef, queue))
            {
                _oldRef = queue;
                _elementsParent.Clear();
                DrawQueue(queue);
            }

            if (_foldout.value == false)
                return;

            if (_drawers.Count != _oldRef.Count)
            {
                _elementsParent.Clear();
                DrawQueue(queue);
            }

            foreach (var drawer in _drawers)
                drawer.Update();
        }
    }
}