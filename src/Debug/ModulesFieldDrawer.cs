using System;

namespace ModulesFrameworkUnity.Debug
{
    public abstract class ModulesFieldDrawer
    {
        public abstract Type PropertyType { get; }
    }
    public abstract class ModulesFieldDrawer<T> : ModulesFieldDrawer
    {
        public override Type PropertyType => typeof(T);
        public abstract void Draw(string fieldName, T fieldValue, int level);
    }
}