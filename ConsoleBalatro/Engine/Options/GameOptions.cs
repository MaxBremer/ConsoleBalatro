using System.Collections.Generic;

namespace ConsoleBalatro.Engine.Options
{
    /// <summary>The ordered catalog consumed by the options UI.</summary>
    public static class GameOptions
    {
        public static IReadOnlyList<GameOption> All { get; } = new List<GameOption>
        {
            new ToggleGameOption("Unique ante tags",
                "Guarantee that the two tags offered during an ante are different.",
                () => Globals.GUARANTEE_UNIQUE_TAGS, value => Globals.GUARANTEE_UNIQUE_TAGS = value),
            new ToggleGameOption("Mirror Illusion seal glitch",
                "Match Balatro's Illusion glitch: shop playing cards cannot receive seals.",
                () => Globals.MIRROR_ILLUSION_SEAL_GLITCH, value => Globals.MIRROR_ILLUSION_SEAL_GLITCH = value),
            new ToggleGameOption("Debug commands",
                "Enable the debug command line and debug-only reroll shortcuts.",
                () => Globals.ALLOW_DEBUG_COMMANDS, value => Globals.ALLOW_DEBUG_COMMANDS = value),
            new ChipLimitGameOption("Maximum chip count",
                "Set the score ceiling. Left/right changes it by a factor of ten; minimum is 2 billion.",
                () => Globals.MaxChipCount, value => Globals.MaxChipCount = value, Globals.MinimumMaxChipCount),
        };
    }
}
