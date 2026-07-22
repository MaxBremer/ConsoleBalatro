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
    private const bool RESET_PROGRESS_ON_START = false;
    private const bool SKIP_TO_BLIND_SELECTION = false;

    public static void Main(string[] args)
    {
        var inte = new Interface(EngineDisplayConstants.INTERFACE_WIDTH, EngineDisplayConstants.INTERFACE_HEIGHT);

        Console.CursorVisible = false;
        Globals.InitializeMain();
        UIStateManager.InitializeUIStateManager();

        //For now, always reset achievement progress to default.
        if (RESET_PROGRESS_ON_START)
        {
            UnlockManager.ResetProgressToDefaults();
            UnlockManager.SaveProgress();
        }

        Console.Clear();
        EngineDisplayGlobals.InitializeDisplayAll(inte);
        if (SKIP_TO_BLIND_SELECTION)
        {
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
        }
        else
        {
            FlowHandler.InitializeMainMenu();
        }

        EngineDisplayGlobals.PlayCachedAnimations();

        while (!Globals.QUIT)
        {
            ControlManager.EngageCurrentControlset(null);
            EngineDisplayGlobals.PlayCachedAnimations();
        }
    }
}
