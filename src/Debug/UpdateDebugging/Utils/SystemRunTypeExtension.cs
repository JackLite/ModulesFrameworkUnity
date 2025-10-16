using System;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging
{
    internal static class SystemRunTypeExtension
    {
        public static SystemRunType Next(this SystemRunType type)
        {
            var nextIndex = (int)type + 1;
            if (nextIndex >= Enum.GetValues(typeof(SystemRunType)).Length)
                nextIndex = 0;

            return (SystemRunType)nextIndex;
        }
    }
}