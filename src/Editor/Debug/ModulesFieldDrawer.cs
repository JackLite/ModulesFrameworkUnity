using System;

namespace ModulesFrameworkUnity.Debug
{
    public abstract class ModulesFieldDrawer
    {
        protected EditorDrawer Drawer { get; set; }

        internal void Init(EditorDrawer drawer)
        {
            Drawer = drawer;
        }
        public abstract Type PropertyType { get; }
        public abstract object Draw(string fieldName, object fieldValue, int level);
    }

    public abstract class ModulesFieldDrawer<T> : ModulesFieldDrawer
    {
        public override Type PropertyType => typeof(T);
        public override object Draw(string fieldName, object fieldValue, int level)
        {
            return Draw(fieldName, (T) fieldValue, level);
        }

        public abstract object Draw(string fieldName, T fieldValue, int level);
    }
}