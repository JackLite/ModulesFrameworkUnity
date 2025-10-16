namespace ModulesFrameworkUnity.Debug.UpdateDebugging
{
    /// <summary>
    ///     Type of system runs in debug mode
    /// </summary>
    internal enum SystemRunType : byte
    {
        RunEvents,
        Run,
        PostRunEvents,
        PostRun,
        FrameEndEvents
    }
}