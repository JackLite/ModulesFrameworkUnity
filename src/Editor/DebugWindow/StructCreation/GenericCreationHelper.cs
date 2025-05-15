using System;
using System.Linq;

namespace ModulesFrameworkUnity.DebugWindow.StructCreation
{
    /// <summary>
    ///     Contains generic arguments type and call event when all types are chosen
    /// </summary>
    public struct GenericCreationHelper
    {
        public Type[] Arguments { get; }

        public event Action OnAllTypesChosen;

        public GenericCreationHelper(int typeCount)
        {
            Arguments = new Type[typeCount];
            OnAllTypesChosen = null;
        }

        public void SetType(int index, Type type)
        {
            Arguments[index] = type;
            if (Arguments.All(t => t != null))
                OnAllTypesChosen?.Invoke();
        }
    }
}