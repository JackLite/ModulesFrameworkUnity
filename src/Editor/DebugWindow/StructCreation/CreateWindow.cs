using System;
using ModulesFramework.Data;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using ModulesFrameworkUnity.DebugWindow.StructCreation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Common
{
    /// <summary>
    ///     Base class for all windows that allows to find some type and choose it
    /// </summary>
    public abstract class CreateWindow : EditorWindow
    {
        protected abstract string RecentTypesKey { get; }

        protected EditorDrawer _mainDrawer;
        protected VisualElement _newStructContainer;
        protected ScrollView _mainContainer;
        protected SearchStructWidget _searchWidget;
        protected VisualElement _genericSearchContainer;
        private StructsDrawer _drawer;

        public virtual void ShowWindow()
        {
            var styles = Resources.Load<StyleSheet>("StructCreationWindow");
            _mainDrawer = new EditorDrawer();
            _mainContainer = new ScrollView();
            _mainContainer.styleSheets.Add(styles);
            _mainContainer.AddToClassList("modules-debug--create-struct");
            rootVisualElement.Add(_mainContainer);
            _searchWidget = new SearchStructWidget();
            _searchWidget.Init(RecentTypesKey, OnChoose);
            _searchWidget.OnSearch += () => _newStructContainer?.Clear();
            _searchWidget.DrawSearch();
            _mainContainer.Add(_searchWidget);

            _genericSearchContainer = new VisualElement();
            _mainContainer.Add(_genericSearchContainer);

            ShowAuxWindow();
        }

        private void Update()
        {
            _drawer?.Update();
        }

        protected virtual void OnChoose(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                ProceedGenericCreation(type, _genericSearchContainer, OnChoose);
                return;
            }

            _genericSearchContainer.Clear();
            if (_newStructContainer == null)
            {
                _newStructContainer = new VisualElement();
                _mainContainer.Add(_newStructContainer);
                _newStructContainer.AddToClassList("modules-debug--create-struct--new-component");
            }
            else
            {
                _newStructContainer.Clear();
            }

            var newData = Activator.CreateInstance(type);
            DrawNewStruct(type, newData, _newStructContainer);

            OnCreateStruct(type, newData);
        }

        private void ProceedGenericCreation(Type type, VisualElement parent, Action<Type> onCreate)
        {
            parent.Clear();
            var genericArguments = type.GetGenericArguments();
            var genericHelper = new GenericCreationHelper(genericArguments.Length);
            genericHelper.OnAllTypesChosen += () =>
            {
                var concreteType = type.MakeGenericType(genericHelper.Arguments);
                onCreate(concreteType);
            };

            for (var i = 0; i < genericArguments.Length; i++)
            {
                var innerContainer = new VisualElement();

                Type argument = genericArguments[i];
                var labelText = $"Choose type for {argument.GetTypeName()} in {type.GetTypeName()}";
                parent.Add(new Label(labelText));
                var search = new SearchStructWidget();
                var argumentPosition = i;
                search.Init(RecentTypesKey, Choose, true);
                search.OnSearch += () => _newStructContainer?.Clear();
                search.DrawSearch();
                parent.Add(search);
                parent.Add(innerContainer);

                continue;

                void Choose(Type genericType)
                {
                    search.ResetSearch();
                    if (!genericType.IsGenericTypeDefinition)
                    {
                        search.Finish(genericType);
                        genericHelper.SetType(argumentPosition, genericType);
                    }
                    else
                    {
                        ProceedGenericCreation(
                            genericType,
                            innerContainer,
                            inner => genericHelper.SetType(argumentPosition, inner)
                        );
                    }
                }
            }
        }

        protected abstract void OnCreateStruct(Type type, object data);

        protected void AddToRecent(Type type)
        {
            _searchWidget.AddToRecent(type);
        }

        protected void DrawNewStruct(Type type, object newComponent, VisualElement parent)
        {
            _drawer = new StructsDrawer();
            _drawer.Init(_mainDrawer, (_, newVal) => { newComponent = newVal; }, () => newComponent);
            _drawer.SetVisible(true);
            _drawer.Draw($"{type.GetTypeName()} (new)", type, newComponent, parent);
            _drawer.Foldout.AddToClassList("modules-debug--create-struct--component-drawer");
            _drawer.Foldout.Q(className: Foldout.inputUssClassName).pickingMode = PickingMode.Ignore;
            _drawer.Foldout.Q(className: Foldout.toggleUssClassName).pickingMode = PickingMode.Ignore;
            _drawer.Foldout.Q(className: Foldout.textUssClassName).pickingMode = PickingMode.Ignore;
            _drawer.Foldout.Q(className: Foldout.checkmarkUssClassName).pickingMode = PickingMode.Ignore;
        }
    }
}