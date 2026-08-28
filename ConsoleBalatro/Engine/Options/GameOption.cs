using System;
using System.Numerics;

namespace ConsoleBalatro.Engine.Options
{
    /// <summary>A single menu-ready setting. Add new settings by registering another instance.</summary>
    public abstract class GameOption
    {
        protected GameOption(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public abstract string DisplayValue { get; }
        public abstract void Change(int direction);
    }

    public sealed class ToggleGameOption : GameOption
    {
        private readonly Func<bool> _get;
        private readonly Action<bool> _set;

        public ToggleGameOption(string name, string description, Func<bool> get, Action<bool> set)
            : base(name, description)
        {
            _get = get;
            _set = set;
        }

        public override string DisplayValue => _get() ? "ON" : "OFF";
        public override void Change(int direction) => _set(!_get());
    }

    public sealed class ChipLimitGameOption : GameOption
    {
        private readonly Func<BigInteger> _get;
        private readonly Action<BigInteger> _set;
        private readonly BigInteger _minimum;

        public ChipLimitGameOption(string name, string description, Func<BigInteger> get,
            Action<BigInteger> set, BigInteger minimum) : base(name, description)
        {
            _get = get;
            _set = set;
            _minimum = minimum;
        }

        public override string DisplayValue => Format(_get());

        public override void Change(int direction)
        {
            if (direction > 0)
                _set(_get() * 10);
            else if (_get() > _minimum)
                _set(BigInteger.Max(_minimum, _get() / 10));
        }

        private static string Format(BigInteger value)
        {
            string[] suffixes = { "", " thousand", " million", " billion", " trillion", " quadrillion", " quintillion", " sextillion" };
            var suffix = 0;
            var scaled = value;
            while (scaled >= 1000 && suffix < suffixes.Length - 1 && scaled % 1000 == 0)
            {
                scaled /= 1000;
                suffix++;
            }
            return scaled + suffixes[suffix];
        }
    }
}
