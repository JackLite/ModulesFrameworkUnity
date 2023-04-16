using System;

namespace ModulesFrameworkUnity.Settings
{
    [Serializable]
    public struct PerformanceSettings
    {
        /// <summary>
        ///     If turn off - only panic messages will be logging
        /// </summary>
        public bool debugMode;
        
        /// <summary>
        ///     First threshold. Use it to get warning when code too long but not critical
        /// </summary>
        public float warningAvgFrameMs;
        
        /// <summary>
        ///     This is the critical threshold. 
        ///     You should ignore it when scene loading or some expected going on.
        ///     It's good to set it to 10-20% from 1/targeted frame rate.
        /// </summary>
        public float panicAvgFrameMs;
    }
}