using ModulesFramework;
using ModulesFrameworkUnity.DebugWindow.Common;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    public class CreateOneDataWindow : CreateWindow
    {
        protected override string RecentTypesKey { get; } = "ModulesDebug.CreateNewOneData.RecentTypes";

        public override void ShowWindow()
        {
            titleContent = new GUIContent("Create One Data");
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
            var newData = Activator.CreateInstance(type);
            DrawNewStruct(type, newData, _newComponentContainer);

            var createBtnContainer = new VisualElement();
            createBtnContainer.AddToClassList("modules-debug--add-component--component-buttons");
            _newComponentContainer.Add(createBtnContainer);

            var createBtn = new Button();
            createBtn.text = "Create";
            createBtn.clicked += () => CreateOneData(type, newData);
            createBtn.AddToClassList("modules-debug--add-component--component-add");
            createBtnContainer.Add(createBtn);
        }

        private void CreateOneData(Type type, object newComponent)
        {
            if (!MF.IsInitialized)
            {
                UnityEngine.Debug.LogError("[Modules.Debug] Modules Framework is not initialized");
                return;
            }

            var methodInfo = GetType().GetMethod(nameof(CreateOneDataWrapper), BindingFlags.NonPublic | BindingFlags.Instance);
            var method = methodInfo.MakeGenericMethod(type);
            method.Invoke(this, new[] { newComponent });
            AddToRecent(type);
        }

        // we need this wrapper because we cannot use ref-return methods by reflection
        private void CreateOneDataWrapper<T>(T data) where T : struct
        {
            MF.World.CreateOneData(data);
        }
    }
}
