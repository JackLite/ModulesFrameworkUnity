using System;
using System.Collections.Generic;

namespace ModulesFrameworkUnity.Debug
{
    /// <summary>
    ///     For display in hierarchy
    /// </summary>
    internal class TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (ReferenceEquals(null, y))
                return 1;
            if (ReferenceEquals(null, x))
                return -1;
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}