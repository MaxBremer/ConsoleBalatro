using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Options;
using ConsoleBalatro.UI.EngineUI;
using System.Collections.Generic;
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

        [Fact]
        public void OptionsMenuPaginatesCatalogsLargerThanItsVisibleRows()
        {
            var options = new List<GameOption>();
            for (var i = 0; i < 10; i++)
            {
                var optionNumber = i;
                options.Add(new ToggleGameOption($"Option {optionNumber}", $"Description {optionNumber}", () => false, _ => { }));
            }
            var menu = new OptionsMenuDisplay(0, 0, options);

            for (var i = 0; i < 9; i++)
                menu.SelectNext();

            var exception = Record.Exception(menu.PreDisplaySetup);

            Assert.Null(exception);
            Assert.Equal(9, menu.SelectedIndex);
            Assert.Equal("Page 3/3", ReadSprite(menu, 98, 14, 8));
            Assert.Equal("> Option 9", ReadSprite(menu, 4, 8, 10));
            Assert.Equal("DESCRIPTION", ReadSprite(menu, 4, 14, 11));
        }

        private static string ReadSprite(OptionsMenuDisplay menu, int x, int y, int length)
        {
            var result = string.Empty;
            for (var i = 0; i < length; i++)
                result += menu.Sprite[y, x + i];
            return result;
        }
    }
}
