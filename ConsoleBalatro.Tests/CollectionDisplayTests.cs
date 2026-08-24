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
            var jimboIndex = display.IndOfJoker("JIMBO");
            while(jimboIndex >= 0 && display.SelectedCardIndex != jimboIndex)
            {
                display.SelectNextCard();
            }

            display.PreDisplaySetup();
            Assert.Contains("JIM", Flatten(display).ToUpper());
            Assert.Contains("JIMBO", Flatten(display).ToUpper());
        }

        [Fact]
        public void ClosingCollectionRestoresRunRandomState()
        {
            ResetEngineForTest();
            Globals.PushGameState(new GameStateObj { GameState = GameState.MainMenu });
            var before = Globals.RunRandom.State;
            var expectedRandom = new RunRandom(before);
            var expectedNextValue = expectedRandom.Next(1_000_000);

            FlowHandler.OpenCollectionMenu();
            _ = Globals.randomNext(1_000_000);
            _ = Globals.randomNext(1_000_000);
            FlowHandler.ClosePlaceholderMenu();

            Assert.Equal(GameState.MainMenu, Globals.CurrentGameState);
            Assert.Equal(before.InitialSeed, Globals.RunRandom.State.InitialSeed);
            Assert.Equal(before.CurrentState, Globals.RunRandom.State.CurrentState);
            Assert.Equal(before.DrawCount, Globals.RunRandom.State.DrawCount);
            Assert.Equal(expectedNextValue, Globals.randomNext(1_000_000));
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
