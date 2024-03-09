using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFrameworkUnity.Debug.Drawers;
using ModulesFrameworkUnity.Debug.Drawers.Special;
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
        private readonly List<FieldDrawer> _defaultDrawers;

        private readonly UnsupportedDrawer _unsupportedDrawer = new();

        private readonly List<FieldDrawer> _createdDrawers = new();

        public int Level { get; private set; }

        public EditorDrawer()
        {
            var drawers = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes())
                where type.IsSubclassOf(typeof(FieldDrawer)) && !type.IsAbstract
                select Activator.CreateInstance(type) as FieldDrawer;

            _defaultDrawers = drawers.OrderBy(d => d.Order).ToList();

            foreach (var drawer in _defaultDrawers)
            {
                _factories.Add(drawer.GetType(), () => Activator.CreateInstance(drawer.GetType()) as FieldDrawer);
            }
        }

        public FieldDrawer Draw(
            string fieldName,
            object fieldValue,
            VisualElement parent,
            Action<object, object> onChanged,
            Func<object> getter,
            bool updateDrawer = true)
        {
            Level++;
            foreach (var defaultDrawer in _defaultDrawers)
            {
                if (!defaultDrawer.CanDraw(fieldValue))
                    continue;
                var d = _factories[defaultDrawer.GetType()]();
                d.Init(this, onChanged, getter);
                d.Draw(fieldName, fieldValue, parent);
                if (updateDrawer)
                    _createdDrawers.Add(d);
                Level--;
                return d;
            }

            _unsupportedDrawer.Draw(fieldName, fieldValue, parent);
            Level--;
            return _unsupportedDrawer;
        }

        public void Update()
        {
            foreach (var drawer in _createdDrawers)
            {
                drawer.Update();
            }
        }
    }
}