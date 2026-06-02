using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public static Dictionary<PlayedHandType, int> NumHandTypePlayedThisRound = new();

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

            ResetNumPlayedPerRoundTracker();

            StartHandCountListeners();
        }

        public static void StartHandCountListeners()
        {
            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.HandPlayDone,
                MyAction = args =>
                {
                    if(args is EngineHandPlayDoneArgs handArgs)
                    {
                        var hType = handArgs.HandTypeThatWasPlayed;
                        HandNumTimesPlayed[hType] += 1;
                    }
                }
            });

            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.StartPlayRoundSetupOver,
                MyAction = _ => ResetNumPlayedPerRoundTracker(),
            });

            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.HandPlayDone,
                MyAction = args => {
                    if(args is EngineHandPlayDoneArgs playArgs)
                        NumHandTypePlayedThisRound[playArgs.HandTypeThatWasPlayed] += 1;
                }
            });
        }

        public static void ResetNumPlayedPerRoundTracker()
        {
            NumHandTypePlayedThisRound.Clear();
            foreach (var k in BaseHandScores.Keys)
            {
                NumHandTypePlayedThisRound.Add(k, 0);
            }
        }

        public static void LevelUpHand(PlayedHandType handType)
        {
            var chipBuff = HandBuffAmounts[handType].Item1;
            var multBuff = HandBuffAmounts[handType].Item2;
            var oldChips = CurrentHandStats[handType].Item1;
            var oldMult = CurrentHandStats[handType].Item2;
            var finalChipVal = CurrentHandStats[handType].Item1 + chipBuff;
            var finalMultVal = CurrentHandStats[handType].Item2 + multBuff;

            //TODO: probably emit event
            CurrentHandStats[handType] = (finalChipVal, finalMultVal);
            var oldLevel = HandLevels[handType];
            HandLevels[handType] += 1;
            var newLevel = HandLevels[handType];

            var args = new EngineHandLevelChangeArgs
            {
                MyContext = new() { Context = EventContextType.HandLevelChange },
                HandTypeLevelling = handType,
                isLevelUp = true,
                oldChipAmount = oldChips,
                newChipAmount = finalChipVal,
                chipChangeAmount = chipBuff,
                oldMultAmount = oldMult,
                newMultAmount = finalMultVal,
                multChangeAmount = multBuff,
                oldLevel = oldLevel,
                newLevel = newLevel,
            };
            EngineEventHandler.TriggerEvent(args);
        }

        public static void LevelDownHand(PlayedHandType handType)
        {
            if (HandLevels[handType] == 1)
                return;
            var chipBuff = HandBuffAmounts[handType].Item1;
            var multBuff = HandBuffAmounts[handType].Item2;
            var oldChips = CurrentHandStats[handType].Item1;
            var oldMult = CurrentHandStats[handType].Item2;
            var finalChipVal = CurrentHandStats[handType].Item1 - chipBuff;
            var finalMultVal = CurrentHandStats[handType].Item2 - multBuff;

            //TODO: probably emit event
            CurrentHandStats[handType] = (finalChipVal, finalMultVal);
            var oldLevel = HandLevels[handType];
            HandLevels[handType] -= 1;
            var newLevel = HandLevels[handType];

            var args = new EngineHandLevelChangeArgs
            {
                MyContext = new() { Context = EventContextType.HandLevelChange },
                HandTypeLevelling = handType,
                isLevelUp = false,
                oldChipAmount = oldChips,
                newChipAmount = finalChipVal,
                chipChangeAmount = chipBuff,
                oldMultAmount = oldMult,
                newMultAmount = finalMultVal,
                multChangeAmount = multBuff,
                oldLevel = oldLevel,
                newLevel = newLevel,
            };
            EngineEventHandler.TriggerEvent(args);
        }

        public static void SetBaseHandScore(PlayedHandType hand)
        {
            var targetScorePair = CurrentHandStats[hand];

            var scoreArgs = new EngineSettingBaseHandScoreArgs() { MyContext = new() { Context = EventContextType.SettingBaseChipsMult}, BaseChipAmount = targetScorePair.Item1, BaseMultAmount = (double)targetScorePair.Item2 };
            EngineEventHandler.TriggerEvent(scoreArgs);

            Globals.CurrentChips = scoreArgs.BaseChipAmount;
            Globals.CurrentMult = scoreArgs.BaseMultAmount;
        }

        public static void FinalPlayChipsCalc()
        {
            var preCalcArgs = new EnginePreFinalGainArgs() { MyContext = new EventContext() { Context = EventContextType.PreFinalGainCheck }, FinalChips = Globals.CurrentChips, FinalMult = Globals.CurrentMult };
            EngineEventHandler.TriggerEvent(preCalcArgs);
            int amountBeingAdded = (int)(preCalcArgs.FinalChips * preCalcArgs.FinalMult);
            var gainArgs = new EngineTotalChipsGainArgs() { AmountBeingGained = amountBeingAdded, MyContext = new EventContext() { Context = EventContextType.TotalChipsGained } };
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
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.TotalChipsReset } });
        }
    }
}
