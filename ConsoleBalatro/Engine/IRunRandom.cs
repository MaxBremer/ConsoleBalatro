using System;

namespace ConsoleBalatro.Engine
{
    /// <summary>
    /// Run-owned random number source. Implementations must expose saveable state so a run can be resumed deterministically.
    /// </summary>
    public interface IRunRandom
    {
        RunRandomState State { get; }

        int Next(int maxValue);

        int Next(int minValue, int maxValue);

        int NextInclusive(int minValue, int maxValue);
    }
}
