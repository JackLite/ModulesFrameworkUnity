using ModulesFramework.Attributes;
using ModulesFramework.Systems;

namespace ModulesFrameworkUnity.Debug
{
    /// <summary>
    ///     Rise update event for debug window
    /// </summary>
    [GlobalSystem]
    public class DebugWindowSystem : IRunSystem
    {
        public void Run()
        {
            DebugEventBus.RiseUpdate();
        }
    }
}