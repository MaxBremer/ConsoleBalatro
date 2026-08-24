using ConsoleBalatro.Engine;
using ConsoleBalatro.UI.EngineUI;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class CollectionDisplayTests : TestClassBase
    {
        [Fact]
        public void CategoryNavigationWrapsAndBackReturnsToCategoryList()
        {
            var display = new CollectionDisplay(0, 0);

            display.SelectPreviousCategory();
            Assert.Equal("Boss Blinds", display.SelectedCategory);

            display.EnterCategory();
            Assert.True(display.IsViewingCategory);
            Assert.False(display.Back());
            Assert.False(display.IsViewingCategory);
            Assert.True(display.Back());
        }

        [Fact]
        public void CollectedJokerIsRenderedAsASelectableCard()
        {
            ResetEngineForTest();
            UnlockManager.AddJokerToCollection("JIMBO", saveImmediately: false);
            var display = new CollectionDisplay(0, 0);

            display.EnterCategory();
            display.PreDisplaySetup();

            Assert.Contains("JIM", Flatten(display));
            Assert.Contains("Jimbo", Flatten(display));
        }

        private static string Flatten(CollectionDisplay display)
        {
            var text = "";
            for (var y = 0; y < display.Height; y++)
                for (var x = 0; x < display.Width; x++)
                    text += display.Sprite[y, x];
            return text;
        }
    }
}
