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
    private const int INTERFACE_WIDTH = 120;
    private const int INTERFACE_HEIGHT = 29;

    private const bool RESET_PROGRESS_ON_START = false;
    private const bool SKIP_TO_BLIND_SELECTION = false;

    public static void Main(string[] args)
    {
        var inte = new Interface(INTERFACE_WIDTH, INTERFACE_HEIGHT);

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
