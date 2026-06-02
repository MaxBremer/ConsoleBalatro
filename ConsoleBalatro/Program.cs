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
        //test stuff
        /*var ra = new Random();
        foreach (var c in ZoneManager.DeckZone.Cards)
        {
            if (ra.Next(1, 4) == 3)
            {
                c.Enhancement = Enhancement.STEEL;
                c.Seal = Seal.RED;
            }
        }*/

        Console.Clear();
        EngineDisplayGlobals.InitializeDisplayAll(inte);
        DeckDb.BecomeDeck("YELLOW");
        FlowHandler.StartNewAnte();
        FlowHandler.InitializeBlindSelectionRound();
        FlowHandler.CurSmallBlindTag = TagType.ECONOMY;
        FlowHandler.CurBigBlindTag = TagType.ECONOMY;
        FlowHandler.CurrentBossBlind = "VERDANT LEAF";

        /*//Test jokers
        ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("MIME"));
        ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("JOLLY JOKER"));
        ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("WRATHFUL JOKER"));
        ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("GOLDEN JOKER"));
        ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("CREDIT CARD"));

        //Test consumables
        ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard("STRENGTH"));
        ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard("WORLD"));

        Globals.EmitMoneyGain(999, null);*/
        Globals.EmitMoneyGain(4, null);//Temp starting money command
        Globals.MaxHandsPerRound += 1;
        EngineDisplayGlobals.PlayCachedAnimations();

        while (!Globals.QUIT)
        {
            ControlManager.EngageCurrentControlset(null);
            EngineDisplayGlobals.PlayCachedAnimations();
        }
    }
}
