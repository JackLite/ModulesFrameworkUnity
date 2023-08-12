using System;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    internal interface IFieldDrawer
    {
        bool TryDraw(Type component, 
            string fieldName, 
            object fieldValue, 
            ref int level, 
            out object newValue);
    }
}