using ModulesFramework;
using ModulesFrameworkUnity.DebugWindow.Common;
using System;
using System.Reflection;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    public class CreateOneDataWindow : CreateWindow
    {
        protected override string RecentTypesKey => "ModulesDebug.CreateNewOneData.RecentTypes";

        public override void ShowWindow()
        {
            titleContent = new GUIContent("Create One Data");
            
            base.ShowWindow();
        }

        protected override void OnCreateStruct(Type type, object data)
        {
            var createBtnContainer = new VisualElement();
            createBtnContainer.AddToClassList("modules-debug--create-struct--component-buttons");
            _newStructContainer.Add(createBtnContainer);

            var createBtn = new Button();
            createBtn.text = "Create";
            createBtn.clicked += () => CreateOneData(type, data);
            createBtn.AddToClassList("modules-debug--create-struct--component-add");
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
            DebugUtils.GetCurrentWorld().CreateOneData(data);
        }
    }
}
