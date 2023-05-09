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
        public abstract void Draw(string fieldName, object fieldValue, int level);
    }

    public abstract class ModulesFieldDrawer<T> : ModulesFieldDrawer
    {
        public override Type PropertyType => typeof(T);
        public override void Draw(string fieldName, object fieldValue, int level)
        {
            Draw(fieldName, (T) fieldValue, level);
        }

        public abstract void Draw(string fieldName, T fieldValue, int level);
    }
}