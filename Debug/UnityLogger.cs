using ModulesFramework;

namespace ModulesFrameworkUnity.Debug
{
    public class UnityLogger : IModulesLogger
    {
        private LogFilter _logFilter = LogFilter.Full;

        public void LogDebug(string msg, LogFilter logFilter)
        {
            if((_logFilter & logFilter) != LogFilter.None)
                UnityEngine.Debug.Log($"[Modules] {msg}");
        }

        public void LogDebug(object msg, LogFilter logFilter)
        {
            if((_logFilter & logFilter) != LogFilter.None)
                UnityEngine.Debug.Log($"[Modules] {msg}");
        }

        public void LogWarning(string msg)
        {
            UnityEngine.Debug.LogWarning($"[Modules] {msg}");
        }

        public void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning($"[Modules] {msg}");
        }

        public void LogError(string msg)
        {
            UnityEngine.Debug.LogError($"[Modules] {msg}");
        }

        public void LogError(object msg)
        {
            UnityEngine.Debug.LogError($"[Modules] {msg}");
        }

        public void SetLogType(LogFilter logFilter)
        {
            _logFilter = logFilter;
        }
    }
}