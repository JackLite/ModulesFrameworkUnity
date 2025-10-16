namespace ModulesFrameworkUnity.Debug
{
    /// <summary>
    ///     Type of module runs in debug mode
    /// </summary>
    internal enum ModuleRunType : byte
    {
        Run,
        PostRun,
        FrameEnd
    }
}