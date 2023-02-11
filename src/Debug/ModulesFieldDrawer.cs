using System;

namespace ModulesFrameworkUnity.Debug
{
    public abstract class ModulesFieldDrawer
    {
        public abstract Type PropertyType { get; }
        public abstract void Draw(string fieldName, object fieldValue, int level);
    }
}