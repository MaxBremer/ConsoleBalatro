using System;

namespace ConsoleBalatro.Engine
{
    /// <summary>
    /// Deterministic, saveable per-run PRNG using SplitMix64 state advancement.
    /// </summary>
    public sealed class RunRandom : IRunRandom
    {
        private const ulong GoldenGamma = 0x9E3779B97F4A7C15UL;
        private ulong _state;

        public RunRandomState State => new()
        {
            InitialSeed = InitialSeed,
            CurrentState = _state,
            DrawCount = DrawCount,
        };

        public ulong InitialSeed { get; }

        public ulong DrawCount { get; private set; }

        public RunRandom(ulong seed)
        {
            InitialSeed = seed;
            _state = seed;
        }

        public RunRandom(RunRandomState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            InitialSeed = state.InitialSeed;
            _state = state.CurrentState;
            DrawCount = state.DrawCount;
        }

        public static RunRandom CreateNewRunRandom()
        {
            Span<byte> seedBytes = stackalloc byte[sizeof(ulong)];
            System.Security.Cryptography.RandomNumberGenerator.Fill(seedBytes);
            return new RunRandom(BitConverter.ToUInt64(seedBytes));
        }

        public int Next(int maxValue)
        {
            if (maxValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be positive.");

            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be greater than minValue.");

            uint range = (uint)(maxValue - minValue);
            return minValue + (int)NextBoundedUInt32(range);
        }

        public int NextInclusive(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be greater than or equal to minValue.");

            if (maxValue == int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "Inclusive ranges ending at int.MaxValue are not supported.");

            return Next(minValue, maxValue + 1);
        }

        private uint NextBoundedUInt32(uint bound)
        {
            ulong threshold = (ulong)(uint.MaxValue - bound + 1) % bound;

            while (true)
            {
                ulong value = NextUInt64() >> 32;
                if (value >= threshold)
                    return (uint)(value % bound);
            }
        }

        private ulong NextUInt64()
        {
            _state += GoldenGamma;
            DrawCount++;

            ulong z = _state;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }
}
