using Modules.Extensions.Prototypes.Editor;
using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Common;
using UnityEditor;
namespace ModulesFrameworkUnity.Utils
{
    public class GoToEntityConnector
    {
        [InitializeOnLoadMethod]
        public static void Subscribe()
        {
            EntityPrototypesEventBus.goToEntityClick -= GoToEntity;
            EntityPrototypesEventBus.goToEntityClick += GoToEntity;
        }
        private static void GoToEntity(Entity entity)
        {
            EditorWindow.GetWindow<DebugWindow>().ChooseEntity(entity);
        }
    }
}
