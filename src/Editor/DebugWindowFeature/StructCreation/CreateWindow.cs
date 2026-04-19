using System;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.StructCreation
{
    /// <summary>
    ///     Base class for all windows that allows to find some type and choose it
    /// </summary>
    public abstract class CreateWindow : EditorWindow
    {
        protected abstract string RecentTypesKey { get; }

        protected EditorDrawer mainDrawer;
        protected VisualElement newStructContainer;
        protected ScrollView mainContainer;
        protected SearchStructWidget searchWidget;
        protected VisualElement genericSearchContainer;
        private StructsDrawer _drawer;

        public virtual void ShowWindow()
        {
            var styles = Resources.Load<StyleSheet>("StructCreationWindow");
            mainDrawer = new EditorDrawer();
            mainContainer = new ScrollView();
            mainContainer.styleSheets.Add(styles);
            mainContainer.AddToClassList("modules-debug--create-struct");
            rootVisualElement.Add(mainContainer);
            searchWidget = new SearchStructWidget();
            searchWidget.Init(RecentTypesKey, OnChoose);
            searchWidget.OnSearch += () => newStructContainer?.Clear();
            searchWidget.DrawSearch();
            mainContainer.Add(searchWidget);

            genericSearchContainer = new VisualElement();
            mainContainer.Add(genericSearchContainer);

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
                ProceedGenericCreation(type, genericSearchContainer, OnChoose);
                return;
            }

            genericSearchContainer.Clear();
            if (newStructContainer == null)
            {
                newStructContainer = new VisualElement();
                mainContainer.Add(newStructContainer);
                newStructContainer.AddToClassList("modules-debug--create-struct--new-component");
            }
            else
            {
                newStructContainer.Clear();
            }

            var newData = Activator.CreateInstance(type);
            DrawNewStruct(type, newData, newStructContainer);

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
                search.OnSearch += () => newStructContainer?.Clear();
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
            searchWidget.AddToRecent(type);
        }

        protected void DrawNewStruct(Type type, object newComponent, VisualElement parent)
        {
            _drawer = new StructsDrawer();
            _drawer.Init(mainDrawer, (_, newVal) => { newComponent = newVal; }, () => newComponent);
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