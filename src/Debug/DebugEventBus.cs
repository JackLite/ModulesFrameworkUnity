using System;

namespace ModulesFrameworkUnity.Debug
{
    public static class DebugEventBus
    {
        public static event Action Update;

        public static void RiseUpdate()
        {
            Update?.Invoke();
        }
    }
}