using System;
using System.Linq;
using System.Reflection;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.DebugWindow.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Events
{
    public class RiseEventWindow : CreateWindow
    {
        protected override string RecentTypesKey { get; } = "ModulesDebug.RiseEvent.RecentTypes";

        public override void ShowWindow()
        {
            titleContent = new GUIContent("Rise event");

            base.ShowWindow();
        }

        protected override void OnChoose(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                DrawChooseGenericTypes(type);
                return;
            }
            if (_newStructContainer == null)
            {
                _newStructContainer = new VisualElement();
                _mainContainer.Add(_newStructContainer);
                _newStructContainer.AddToClassList("modules-debug--add-component--new-component");
            }
            else
            {
                _newStructContainer.Clear();
            }

            var ev = Activator.CreateInstance(type);
            DrawNewStruct(type, ev, _newStructContainer);

            var addButtonsContainer = new VisualElement();
            addButtonsContainer.AddToClassList("modules-debug--add-component--component-buttons");
            _newStructContainer.Add(addButtonsContainer);

            var addBtn = new Button();
            addBtn.text = "Rise";
            addBtn.clicked += () => RiseEvent(type, ev);
            addBtn.AddToClassList("modules-debug--add-component--component-add");
            addButtonsContainer.Add(addBtn);
        }

        private void DrawChooseGenericTypes(Type type)
        {
            var targetType = type.MakeGenericType(typeof(string));
            OnChoose(targetType);
        }

        private void RiseEvent(Type type, object ev)
        {
            var world = DebugUtils.GetCurrentWorld();
            var rise = world.GetType()
                .GetMethods()
                .First(m => m.Name == nameof(world.RiseEvent) && m.GetParameters().Length == 1)
                .MakeGenericMethod(type);

            rise.Invoke(world, new[] { ev });
            AddToRecent(type);
        }
    }
}