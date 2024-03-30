using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug.Drawers;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    [CustomEditor(typeof(EntityViewer))]
    public class EntityViewerEditor : Editor
    {
        private EntityViewer _viewer;
        private readonly EditorDrawer _drawer = new EditorDrawer();
        private VisualElement _root;
        private VisualElement _components;

        /// <summary>
        ///     Foldout elements per component type
        /// </summary>
        private readonly Dictionary<Type, Foldout> _componentsElements = new();

        /// <summary>
        ///     Drawers per component
        /// </summary>
        private readonly Dictionary<Type, Dictionary<int, FieldDrawer>> _drawers = new();

        /// <summary>
        ///     Set of components that we should remove from inspector
        /// </summary>
        private readonly HashSet<Type> _removeElements = new();

        private FoldoutPool _foldoutPool;
        private readonly Comparison<VisualElement> _elementComparison = ElementsComparer;

        /// <summary>
        ///     If true then foldouts will resort every time when components set is changed
        /// </summary>
        private bool _resortAlways;

        private void OnEnable()
        {
            _viewer = (EntityViewer)serializedObject.targetObject;
            _drawer.Clear();
            _viewer.OnComponentsSetChanged += Redraw;
            _viewer.OnUpdate += _drawer.Update;
        }

        private void OnDisable()
        {
            _viewer.OnComponentsSetChanged -= Redraw;
            _viewer.OnUpdate -= _drawer.Update;
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            DrawResortFlag();
            _components = new VisualElement();
            _root.Add(_components);
            _foldoutPool = new FoldoutPool(_viewer.components.Values.SelectMany(p => p).Count(), _components);
            Draw();
            return _root;
        }

        private void DrawResortFlag()
        {
            var resortToggle = new Toggle();
            resortToggle.text = "Resort always";
            resortToggle.value = _resortAlways;
            resortToggle.style.marginBottom = 15;
            resortToggle.style.marginTop = 5;
            resortToggle.RegisterValueChangedCallback(evt =>
            {
                _resortAlways = evt.newValue;
            });
            _root.Add(resortToggle);
        }

        private void Draw()
        {
            foreach (var kvp in _viewer.components)
            {
                DrawOneComponent(kvp.Key, kvp.Value);
            }

            _components.Sort(_elementComparison);
        }

        private static int ElementsComparer(VisualElement el1, VisualElement el2)
        {
            if (el1.style.opacity.value > el2.style.opacity.value)
                return -1;
            if (el1.style.opacity.value < el2.style.opacity.value)
                return 1;

            if (el1 is not Foldout f1 || el2 is not Foldout f2)
                return 0;
            return string.Compare(f1.text, f2.text, StringComparison.Ordinal);
        }

        private void Redraw()
        {
            RemoveNonExistedTypes();
            AddNewSingle();

            RedrawMultipleIfNeed();
            if (_resortAlways)
                _components.Sort(_elementComparison);
        }

        private void RedrawMultipleIfNeed()
        {
            foreach (var (type, components) in _viewer.components)
            {
                if (components.Count <= 1)
                    continue;

                if (_drawers.ContainsKey(type) && _drawers[type].Count == components.Count)
                    continue;

                if (_drawers.TryGetValue(type, out var drawers))
                {
                    foreach (var (_, drawer) in drawers)
                    {
                        _drawer.RemoveDrawer(drawer);
                    }

                    drawers.Clear();
                }

                if (_componentsElements.TryGetValue(type, out var fd))
                {
                    _foldoutPool.Return(fd);
                    _componentsElements.Remove(type);
                }

                DrawOneComponent(type, components);
            }
        }

        private void AddNewSingle()
        {
            foreach (var kvp in _viewer.components)
            {
                if (_componentsElements.ContainsKey(kvp.Key))
                    continue;
                DrawOneComponent(kvp.Key, kvp.Value);
            }
        }

        private void RemoveNonExistedTypes()
        {
            _removeElements.Clear();
            foreach (var type in _componentsElements.Keys)
                _removeElements.Add(type);

            foreach (var key in _viewer.components.Keys)
                _removeElements.Remove(key);

            foreach (var type in _removeElements)
            {
                if (!_componentsElements.TryGetValue(type, out var element))
                    continue;
                _foldoutPool.Return(element);
                if (_drawers.TryGetValue(type, out var drawers))
                {
                    foreach (var (_, drawer) in drawers)
                    {
                        _drawer.RemoveDrawer(drawer);
                    }
                }

                _drawers.Remove(type);
                _componentsElements.Remove(type);
            }
        }

        private void DrawOneComponent(Type type, ICollection components)
        {
            var componentFoldout = _foldoutPool.Pop();
            componentFoldout.Clear();
            componentFoldout.style.opacity = 1;
            _componentsElements.Add(type, componentFoldout);

            var fieldName = type.Name;
            if (components.Count > 1)
                fieldName += $"({components.Count.ToString(CultureInfo.InvariantCulture)})";

            componentFoldout.text = fieldName;

            var fields = type.GetFields();
            componentFoldout.RegisterValueChangedCallback(ev =>
            {
                if(ev.newValue && componentFoldout.childCount == 0)
                    DrawFields(type, componentFoldout);
            });

            if (fields.Length == 0 || !componentFoldout.value)
                return;

            DrawFields(type, componentFoldout);
        }

        private void DrawFields(Type type, Foldout componentFoldout)
        {
            var number = 0;
            foreach (var (index, _) in _viewer.components[type])
            {
                var origin = _viewer.components[type][index];
                var componentName = type.Name;
                var ecsTable = _viewer.World.GetEcsTable(type);
                if (ecsTable.IsMultiple)
                    componentName = $"{componentName}[{number++}]";

                var drawer = _drawer.Draw(componentName, origin, componentFoldout, (_, newVal) =>
                    {
                        _viewer.changedComponents[type][index] = newVal;
                        ApplyChanges(ecsTable, type);
                    },
                    () =>
                    {
                        if (!_viewer.components.TryGetValue(type, out var components))
                            return Activator.CreateInstance(type);
                        if (!components.TryGetValue(index, out var comp))
                            return Activator.CreateInstance(type);
                        return comp;
                    });

                if (_drawers.TryGetValue(type, out var drawers))
                {
                    drawers.Add(index, drawer);
                }
                else
                {
                    _drawers.Add(type, new Dictionary<int, FieldDrawer>
                    {
                        {
                            index, drawer
                        }
                    });
                }
            }
        }

        private void ApplyChanges(EcsTable ecsTable, Type type)
        {
            if (ecsTable.IsMultiple)
                ecsTable.SetDataObjects(_viewer.Eid, _viewer.changedComponents[type].Values.ToList());
            else
                ecsTable.SetDataObject(_viewer.Eid, _viewer.changedComponents[type].Values.First());
        }
    }
}