using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Options;
using System.Linq;
using System.Numerics;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class GameOptionsTests
    {
        [Fact]
        public void CatalogContainsTheFourSupportedSettings()
        {
            Assert.Equal(4, GameOptions.All.Count);
            Assert.Contains(GameOptions.All, option => option.Name == "Unique ante tags");
            Assert.Contains(GameOptions.All, option => option.Name == "Mirror Illusion seal glitch");
            Assert.Contains(GameOptions.All, option => option.Name == "Debug commands");
            Assert.Contains(GameOptions.All, option => option.Name == "Maximum chip count");
        }

        [Fact]
        public void ToggleOptionUpdatesItsGlobalSetting()
        {
            var option = GameOptions.All.Single(option => option.Name == "Unique ante tags");
            var original = Globals.GUARANTEE_UNIQUE_TAGS;

            option.Change(1);

            Assert.Equal(!original, Globals.GUARANTEE_UNIQUE_TAGS);
            option.Change(1);
            Assert.Equal(original, Globals.GUARANTEE_UNIQUE_TAGS);
        }

        [Fact]
        public void MaximumChipCountCannotGoBelowTwoBillion()
        {
            var original = Globals.MaxChipCount;
            try
            {
                Globals.MaxChipCount = BigInteger.One;
                Assert.Equal(new BigInteger(2_000_000_000), Globals.MaxChipCount);

                var option = GameOptions.All.Single(option => option.Name == "Maximum chip count");
                option.Change(-1);
                Assert.Equal(new BigInteger(2_000_000_000), Globals.MaxChipCount);
            }
            finally
            {
                Globals.MaxChipCount = original;
            }
        }
    }
}
