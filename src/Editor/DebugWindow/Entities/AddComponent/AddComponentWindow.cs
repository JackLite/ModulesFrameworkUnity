using ModulesFrameworkUnity.DebugWindow.Common;
using System;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.EntitiesTags;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Entities.AddComponent
{
    public class AddComponentWindow : CreateWindow
    {
        protected int _eid;

        protected override string RecentTypesKey { get; } = "ModulesDebug.AddComponent.RecentTypes";

        public void Show(int eid)
        {
            _eid = eid;
            var entity = DebugUtils.GetCurrentWorld().GetEntity(eid);
            var tags = entity.GetEntityTagsAsString();
            var entityTitle = tags == string.Empty ? entity.GetCustomId() : tags;
            titleContent = new GUIContent($"Add component to {entityTitle}");

            base.ShowWindow();
        }

        protected override void OnCreateStruct(Type type, object data)
        {
            var addButtonsContainer = new VisualElement();
            addButtonsContainer.AddToClassList("modules-debug--create-struct--component-buttons");
            _newStructContainer.Add(addButtonsContainer);

            var addBtn = new Button();
            addBtn.text = "Add";
            addBtn.clicked += () => AddComponent(type, data);
            addBtn.AddToClassList("modules-debug--create-struct--component-add");
            addButtonsContainer.Add(addBtn);

            var addMultipleBtn = new Button();
            addMultipleBtn.text = "Add as multiple";
            addMultipleBtn.clicked += () => AddMultipleComponent(type, data);
            addMultipleBtn.AddToClassList("modules-debug--create-struct--component-add");
            addButtonsContainer.Add(addMultipleBtn);
        }

        private void AddComponent(Type type, object component)
        {
            DebugUtils.GetCurrentWorld().AddComponent(_eid, type, component);
            AddToRecent(type);
        }

        private void AddMultipleComponent(Type type, object component)
        {
            DebugUtils.GetCurrentWorld().AddNewComponent(_eid, type, component);
            AddToRecent(type);
        }
    }
}
