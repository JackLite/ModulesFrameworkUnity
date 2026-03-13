using System;
using ModulesFramework.Systems;

namespace ModulesFramework.Systems
{
    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IPostRunSystem")]
    public interface IPostRunSystem : ISystem
    {
        public void PostRun();
    }
}

namespace ModulesFrameworkUnity.Systems
{
    public interface IPostRunSystem : ISystem
    {
        public void PostRun();
    }
}