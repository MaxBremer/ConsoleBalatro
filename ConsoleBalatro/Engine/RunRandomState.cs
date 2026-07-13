namespace ConsoleBalatro.Engine
{
    /// <summary>
    /// Serializable random state intended to be embedded in future RunSaveData.
    /// </summary>
    public sealed class RunRandomState
    {
        public ulong InitialSeed { get; set; }

        public ulong CurrentState { get; set; }

        public ulong DrawCount { get; set; }
    }
}
