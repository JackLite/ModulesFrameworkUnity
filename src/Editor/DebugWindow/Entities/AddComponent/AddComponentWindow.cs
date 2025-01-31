using ModulesFramework;
using ModulesFrameworkUnity.DebugWindow.Common;
using System;
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
            var entity = MF.World.GetEntity(eid);
            var tags = entity.GetEntityTagsAsString();
            var entityTitle = tags == string.Empty ? entity.GetCustomId() : tags;
            titleContent = new GUIContent($"Add component to {entityTitle}");

            base.ShowWindow();
        }

        protected override void OnChoose(Type type)
        {
            if (_newComponentContainer == null)
            {
                _newComponentContainer = new VisualElement();
                _mainContainer.Add(_newComponentContainer);
                _newComponentContainer.AddToClassList("modules-debug--add-component--new-component");
            }
            else
            {
                _newComponentContainer.Clear();
            }
            var newComponent = Activator.CreateInstance(type);
            DrawNewStruct(type, newComponent, _newComponentContainer);

            var addButtonsContainer = new VisualElement();
            addButtonsContainer.AddToClassList("modules-debug--add-component--component-buttons");
            _newComponentContainer.Add(addButtonsContainer);

            var addBtn = new Button();
            addBtn.text = "Add";
            addBtn.clicked += () => AddComponent(type, newComponent);
            addBtn.AddToClassList("modules-debug--add-component--component-add");
            addButtonsContainer.Add(addBtn);

            var addMultipleBtn = new Button();
            addMultipleBtn.text = "Add as multiple";
            addMultipleBtn.clicked += () => AddMultipleComponent(type, newComponent);
            addMultipleBtn.AddToClassList("modules-debug--add-component--component-add");
            addButtonsContainer.Add(addMultipleBtn);
        }

        private void AddComponent(Type type, object component)
        {
            MF.World.AddComponent(_eid, type, component);
            AddToRecent(type);
        }

        private void AddMultipleComponent(Type type, object component)
        {
            MF.World.AddNewComponent(_eid, type, component);
            AddToRecent(type);
        }
    }
}
