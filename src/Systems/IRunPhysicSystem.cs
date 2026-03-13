using System;
using ModulesFramework.Systems;

namespace ModulesFramework.Systems
{
    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IRunPhysicSystem")]
    public interface IRunPhysicSystem : ISystem
    {
        public void RunPhysic();
    }
}

namespace ModulesFrameworkUnity.Systems
{
    public interface IRunPhysicSystem : ISystem
    {
        public void RunPhysic();
    }
}

