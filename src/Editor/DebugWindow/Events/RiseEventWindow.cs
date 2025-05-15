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

        protected override void OnCreateStruct(Type type, object data)
        {
            var addButtonsContainer = new VisualElement();
            addButtonsContainer.AddToClassList("modules-debug--create-struct--component-buttons");
            _newStructContainer.Add(addButtonsContainer);

            var addBtn = new Button();
            addBtn.text = "Rise";
            addBtn.clicked += () => RiseEvent(type, data);
            addBtn.AddToClassList("modules-debug--create-struct--component-add");
            addButtonsContainer.Add(addBtn);
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