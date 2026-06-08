// See https://aka.ms/new-console-template for more information
using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.UI;
using ConsoleBalatro.UI.EngineUI;
using ConsoleBalatro.UI.EngineUI.Controls;

class Program
{
    public static void Main(string[] args)
    {
        var inte = new Interface(120, 29);

        Console.CursorVisible = false;
        Globals.InitializeMain();
        UIStateManager.InitializeUIStateManager();

        Console.Clear();
        EngineDisplayGlobals.InitializeDisplayAll(inte);
        DeckDb.BecomeDeck("YELLOW");
        FlowHandler.StartNewAnte();
        FlowHandler.InitializeBlindSelectionRound();
        //FlowHandler.InitializeDeckSelectionRound();

        EngineDisplayGlobals.PlayCachedAnimations();

        while (!Globals.QUIT)
        {
            ControlManager.EngageCurrentControlset(null);
            EngineDisplayGlobals.PlayCachedAnimations();
        }
    }
}
