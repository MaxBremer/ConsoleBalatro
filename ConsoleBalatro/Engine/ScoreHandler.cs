using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class ScoreHandler
    {
        // It always goes Chips X Mult
        public static Dictionary<PlayedHandType, (int, int)> BaseHandScores = new()
        {
            { PlayedHandType.HIGHCARD, (5, 1) },
            { PlayedHandType.PAIR, (10, 2) },
            { PlayedHandType.TWOPAIR, (20, 2) },
            { PlayedHandType.THREEOFAKIND, (30, 3) },
            { PlayedHandType.STRAIGHT, (30, 4) },
            { PlayedHandType.FLUSH, (35, 4) },
            { PlayedHandType.FULLHOUSE, (40, 4) },
            { PlayedHandType.FOUROFAKIND, (60, 7) },
            { PlayedHandType.STRAIGHTFLUSH, (100, 8) },
            { PlayedHandType.FIVEOFAKIND, (120, 12) },
            { PlayedHandType.FLUSHHOUSE, (140, 14) },
            { PlayedHandType.FLUSHFIVE, (160, 16) },
        };

        public static Dictionary<PlayedHandType, int> HandLevels = new();

        public static Dictionary<PlayedHandType, int> HandNumTimesPlayed = new();

        public static PlayedHandType MostPlayedHand => HandNumTimesPlayed.OrderByDescending(kvp => kvp.Value).First().Key;

        public static Dictionary<PlayedHandType, (int, int)> HandBuffAmounts = new()
        {
            { PlayedHandType.HIGHCARD, (10, 1) },
            { PlayedHandType.PAIR, (15, 1) },
            { PlayedHandType.TWOPAIR, (20, 1) },
            { PlayedHandType.THREEOFAKIND, (20, 2) },
            { PlayedHandType.STRAIGHT, (30, 3) },
            { PlayedHandType.FLUSH, (15, 2) },
            { PlayedHandType.FULLHOUSE, (25, 2) },
            { PlayedHandType.FOUROFAKIND, (30, 3) },
            { PlayedHandType.STRAIGHTFLUSH, (40, 4) },
            { PlayedHandType.FIVEOFAKIND, (35, 3) },
            { PlayedHandType.FLUSHHOUSE, (40, 4) },
            { PlayedHandType.FLUSHFIVE, (50, 3) },
        };

        public static Dictionary<PlayedHandType, (int, int)> CurrentHandStats = new();

        public static void InitializeHandStatTracker()
        {
            CurrentHandStats.Clear();
            HandLevels.Clear();
            HandNumTimesPlayed.Clear();
            foreach (var k in BaseHandScores.Keys)
            {
                CurrentHandStats.Add(k, BaseHandScores[k]);
                HandLevels.Add(k, 1);
                HandNumTimesPlayed.Add(k, 0);
            }

            StartHandCountListeners();
        }

        public static void StartHandCountListeners()
        {
            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.HandPlayedCalculated,
                MyAction = args =>
                {
                    if(args is EngineHandPlayArgs handArgs)
                    {
                        var hType = handArgs.HandBeingPlayed;
                        HandNumTimesPlayed[hType] += 1;
                    }
                }
            });
        }

        public static void LevelUpHand(PlayedHandType handType)
        {
            var chipBuff = HandBuffAmounts[handType].Item1;
            var multBuff = HandBuffAmounts[handType].Item2;
            var finalChipVal = CurrentHandStats[handType].Item1 + chipBuff;
            var finalMultVal = CurrentHandStats[handType].Item2 + multBuff;

            //TODO: probably emit event
            CurrentHandStats[handType] = (finalChipVal, finalMultVal);
            HandLevels[handType] += 1;
        }

        public static void SetBaseHandScore(PlayedHandType hand)
        {
            var targetScorePair = CurrentHandStats[hand];
            Globals.CurrentChips = targetScorePair.Item1;
            Globals.CurrentMult = targetScorePair.Item2;
        }

        public static void FinalPlayChipsCalc()
        {
            int amountBeingAdded = (int)(Globals.CurrentChips * Globals.CurrentMult);
            var gainArgs = new EngineTotalChipsGainArgs() { AmountBeingGained = amountBeingAdded, MyContext = new Events.EventContext() { Context = Events.EventContextType.TotalChipsGained } };
            EngineEventHandler.TriggerEvent(gainArgs);
            Globals.TotalCurrentChips += gainArgs.AmountBeingGained;

            Globals.CurrentMult = 0;
            Globals.CurrentChips = 0;
        }

        public static void ResetScoresPostRound()
        {
            Globals.CurrentChips = 0;
            Globals.CurrentMult = 0;
            Globals.TotalCurrentChips = 0;
        }
    }
}
