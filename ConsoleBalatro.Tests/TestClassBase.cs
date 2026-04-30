using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Tests
{
    public class TestClassBase
    {
        public void ResetEngineForTest()
        {
            EngineEventHandler.ResetFullEventHandler();

            Globals.ResetGlobalValues();
            Globals.ClearGameStateStack();
            Globals.InitializeMain();
            Globals.ClearGameStateStack();
            Globals.ResetGlobalValues();

            FlowHandler.CurrentAnte = 0;
            FlowHandler.CurrentSelectedBlind = BlindType.SMALL;
            FlowHandler.CurrentTempChanges = null;
            FlowHandler.CurrentBossBlind = BossBlindDb.BossBlindNames.First();

            Globals.Money = 0;
            Globals.CurMaxInterest = 5;
            Globals.SetStartOfRoundStats();
            Globals.RequiredChipsForCurrentBlind = -1;

            ZoneManager.HiddenBlindAttributeZone.ClearCards(true);
        }

        public void ResetToBlindSelection()
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
        }
    }
}
