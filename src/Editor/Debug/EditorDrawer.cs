using ModulesFrameworkUnity.Debug.Drawers;
using ModulesFrameworkUnity.Debug.Drawers.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFrameworkUnity.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    public class EditorDrawer
    {
        private readonly Dictionary<Type, Func<FieldDrawer>> _factories = new();

        /// <summary>
        ///     All drawers. We need this collection to check
        ///     if drawer can draw some type to find what drawer we need to create
        /// </summary>
        private static List<FieldDrawer> _defaultDrawers;

        private readonly UnsupportedDrawer _unsupportedDrawer = new();

        private readonly List<FieldDrawer> _createdDrawers = new();

        private int _level;

        public EditorDrawer()
        {
            if (_defaultDrawers == null)
            {
                var drawers = from type in AssemblyUtils.GetAssemblies()
                        .SelectMany(asm => asm.GetTypes())
                              where type.IsSubclassOf(typeof(FieldDrawer)) && !type.IsAbstract
                              select Activator.CreateInstance(type) as FieldDrawer;

                _defaultDrawers = drawers.OrderBy(d => d.Order).ToList();
            }

            foreach (var drawer in _defaultDrawers)
            {
                _factories.Add(drawer.GetType(), () => Activator.CreateInstance(drawer.GetType()) as FieldDrawer);
            }
        }

        public FieldDrawer Draw(
            string fieldName,
            Type fieldType,
            object fieldValue,
            VisualElement parent,
            Action<object, object> onChanged,
            Func<object> getter,
            int level,
            bool updateDrawer = true)
        {
            parent.styleSheets.Add(Resources.Load<StyleSheet>("Drawers"));
            foreach (var defaultDrawer in _defaultDrawers)
            {
                if (!defaultDrawer.CanDraw(fieldType, fieldValue))
                    continue;
                var d = _factories[defaultDrawer.GetType()]();
                d.Level = level;
                d.Init(this, onChanged, getter);
                d.Draw(fieldName, fieldType, fieldValue, parent);
                if (updateDrawer)
                    _createdDrawers.Add(d);
                return d;
            }

            _unsupportedDrawer.Draw(fieldName, fieldType, fieldValue, parent);
            return _unsupportedDrawer;
        }

        public FieldDrawer Draw(
            string fieldName,
            Type fieldType,
            object fieldValue,
            VisualElement parent,
            Action<object, object> onChanged,
            Func<object> getter,
            bool updateDrawer = true)
        {
            _level++;
            var drawer = Draw(fieldName, fieldType, fieldValue, parent, onChanged, getter, _level, updateDrawer);
            _level--;
            return drawer;
        }

        public void Update()
        {
            for (var index = 0; index < _createdDrawers.Count; index++)
            {
                var drawer = _createdDrawers[index];
                drawer.Update();
            }
        }

        public void Clear()
        {
            _createdDrawers.Clear();
        }

        public void RemoveDrawer(FieldDrawer drawer)
        {
            _createdDrawers.Remove(drawer);
        }
    }
}