using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Stakes;
using ConsoleBalatro.UI.EngineUI.Controls;
using Xunit;

namespace ConsoleBalatro.Tests;

public class UnlockManagerTests : TestClassBase
{
    [Fact]
    public void DeckUnlocks_SaveAndLoad_WithDefaultDecks()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            Assert.True(DeckDb.IsDeckUnlocked("RED"));
            Assert.False(DeckDb.IsDeckUnlocked("BLUE"));

            Assert.True(DeckDb.UnlockDeck("BLUE"));
            Assert.True(File.Exists(savePath));

            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            Assert.False(DeckDb.IsDeckUnlocked("BLUE"));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(DeckDb.IsDeckUnlocked("RED"));
            Assert.True(DeckDb.IsDeckUnlocked("BLUE"));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void DeckStakeProgress_IsPerDeck_UnlocksNextStakeAndPersists()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeckDb.UnlockDeck("BLUE");

            Assert.True(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.WHITE));
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.RED));
            Assert.Equal(0, UnlockManager.GetStakesBeatenCountForDeck("RED"));

            Assert.True(UnlockManager.MarkDeckStakeBeaten("RED", StakeType.WHITE));

            Assert.True(UnlockManager.HasDeckStakeSticker("RED", StakeType.WHITE));
            Assert.True(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.RED));
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.GREEN));
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("BLUE", StakeType.RED));
            Assert.Equal(1, UnlockManager.GetStakesBeatenCountForDeck("RED"));

            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.RED));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(UnlockManager.IsStakeUnlockedForDeck("RED", StakeType.RED));
            Assert.True(UnlockManager.HasDeckStakeSticker("RED", StakeType.WHITE));
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("BLUE", StakeType.RED));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void JokerStakeProgress_TracksHighestBeatenStakeAndPersists()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            Assert.Null(UnlockManager.GetHighestBeatenStakeForJoker("JIMBO"));
            Assert.False(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.WHITE));

            Assert.True(UnlockManager.MarkJokerStakeBeaten("JIMBO", StakeType.RED));

            Assert.Equal(StakeType.RED, UnlockManager.GetHighestBeatenStakeForJoker("JIMBO"));
            Assert.True(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.WHITE));
            Assert.True(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.RED));
            Assert.False(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.GREEN));

            Assert.False(UnlockManager.MarkJokerStakeBeaten("JIMBO", StakeType.WHITE));
            Assert.False(UnlockManager.MarkJokerStakeBeaten("NOT A JOKER", StakeType.GOLD));

            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            Assert.Null(UnlockManager.GetHighestBeatenStakeForJoker("JIMBO"));

            Assert.True(UnlockManager.LoadProgress());
            Assert.Equal(StakeType.RED, UnlockManager.GetHighestBeatenStakeForJoker("JIMBO"));
            Assert.True(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.RED));
            Assert.False(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.GREEN));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void WinningAnteEightBoss_AddsStakeStickersToJokersInJokerZone()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            ResetEngineForTest();
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            FlowHandler.CurrentDeckDbName = "RED";
            FlowHandler.CurrentAnte = 8;
            FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
            StakeManager.CurrentStake = StakeType.GREEN;
            ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("JIMBO"));
            ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard("JOLLY JOKER"));
            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard("FOOL"));

            FlowHandler.IncrementBlind();

            Assert.True(UnlockManager.HasDeckStakeSticker("RED", StakeType.GREEN));
            Assert.True(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.GREEN));
            Assert.True(UnlockManager.HasJokerStakeSticker("JOLLY JOKER", StakeType.GREEN));
            Assert.False(UnlockManager.HasJokerStakeSticker("ZANY JOKER", StakeType.GREEN));

            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            Assert.False(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.GREEN));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(UnlockManager.HasJokerStakeSticker("JIMBO", StakeType.GREEN));
            Assert.True(UnlockManager.HasJokerStakeSticker("JOLLY JOKER", StakeType.GREEN));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            ResetEngineForTest();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void WinningAnteEightBoss_PresentsWinChoiceBeforeContinuingRun()
    {
        ResetEngineForTest();
        UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

        FlowHandler.CurrentDeckDbName = "RED";
        FlowHandler.CurrentAnte = 8;
        FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
        FlowHandler.RunWinDecisionPending = false;
        FlowHandler.HasWonCurrentRun = false;

        FlowHandler.IncrementBlind();

        Assert.True(FlowHandler.HasWonCurrentRun);
        Assert.True(FlowHandler.RunWinDecisionPending);
        Assert.Equal(9, FlowHandler.CurrentAnte);
        Assert.Equal(BlindType.SMALL, FlowHandler.CurrentSelectedBlind);

        FlowHandler.InitializePostRound(new List<(string, int)>());
        FlowHandler.ClosePostRound();

        Assert.Equal(GameState.WinMenu, Globals.CurrentGameState);

        FlowHandler.ContinueWonRun();

        Assert.False(FlowHandler.RunWinDecisionPending);
        Assert.Equal(GameState.BlindsMenu, Globals.CurrentGameState);
        Assert.Equal(9, FlowHandler.CurrentAnte);
        Assert.Equal(BlindType.SMALL, FlowHandler.CurrentSelectedBlind);
    }

    //This test is out of date, a) base antes go beyond 12 b) they continue scaling by 2x after the list empties.
    /*[Fact]
    public void AnteScaling_ReusesHighestConfiguredAmountAfterAnteTwelve()
    {
        ResetEngineForTest();
        StakeManager.CurrentStake = StakeType.WHITE;

        FlowHandler.CurrentAnte = 12;
        var anteTwelveAmount = FlowHandler.CurrentBaseChipAmount;

        FlowHandler.CurrentAnte = 13;

        Assert.Equal(anteTwelveAmount, FlowHandler.CurrentBaseChipAmount);
    }*/

    [Fact]
    public void DebugUnlockDeck_UnlocksDeckAndStakeProgress()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            DebugManager.ReadCommand("unlockdeck BLUE GREEN");

            Assert.True(DeckDb.IsDeckUnlocked("BLUE"));
            Assert.True(UnlockManager.IsStakeUnlockedForDeck("BLUE", StakeType.GREEN));
            Assert.False(UnlockManager.IsStakeUnlockedForDeck("BLUE", StakeType.BLACK));
            Assert.True(UnlockManager.HasDeckStakeSticker("BLUE", StakeType.WHITE));
            Assert.True(UnlockManager.HasDeckStakeSticker("BLUE", StakeType.RED));
            Assert.False(UnlockManager.HasDeckStakeSticker("BLUE", StakeType.GREEN));

            DebugManager.ReadCommand("unlockdeck BLUE GREEN -beaten");

            Assert.True(UnlockManager.IsStakeUnlockedForDeck("BLUE", StakeType.BLACK));
            Assert.True(UnlockManager.HasDeckStakeSticker("BLUE", StakeType.GREEN));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void AchievementListener_AchievesOnce_SavesAndDoesNotRelistenAfterLoad()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        var conditionChecks = 0;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            UnlockManager.RegisterAchievementListener(
                "PLAY_A_HAND",
                EventContextType.HandPlayDone,
                _ =>
                {
                    conditionChecks++;
                    return true;
                });

            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });

            Assert.True(UnlockManager.IsAchievementAchieved("PLAY_A_HAND"));
            Assert.Equal(1, conditionChecks);
            Assert.True(File.Exists(savePath));

            UnlockManager.LoadProgress();
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });

            Assert.True(UnlockManager.IsAchievementAchieved("PLAY_A_HAND"));
            Assert.Equal(1, conditionChecks);
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void BuiltInAchievements_TriggerFromDifferentEvents()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        var unlockedPopups = new List<EngineAchievementUnlockArgs>();
        Action<EngineEventArgs> CaptureUnlock = (EngineEventArgs args) => 
        { 
            if (args is EngineAchievementUnlockArgs achArgs) 
            { 
                unlockedPopups.Add(achArgs); 
            } 
        };
        var listener = new EngineEventListener() { MyAction = CaptureUnlock, MyContextType = EventContextType.AchievementUnlocked };
        try
        {
            //UnlockManager.AchievementUnlocked += CaptureUnlock;
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            EngineEventHandler.ResetFullEventHandler();
            UnlockManager.ResetProgressToDefaults();
            EngineEventHandler.ResetSavedEvents();
            EngineEventHandler.StartListening(listener);

            for (var i = 0; i < 9; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.TenHandsPlayedAchievementId));

            EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.TenHandsPlayedAchievementId));

            EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs
            {
                MyContext = new EventContext { Context = EventContextType.HandPlayDone },
                CurrentTotalChips = 10000,
            });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.ScoreTenThousandAchievementId));

            EngineEventHandler.TriggerEvent(new EngineDiscardDoneArgs
            {
                MyContext = new EventContext { Context = EventContextType.HandDiscardDone },
                BeingDiscarded = CardFactory.CardListFromDefString("AS,KS,QS,JS,1S", ","),
            });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Brainstorm_UnlockId));

            Assert.Contains(unlockedPopups, args => args.AchievementName == "Practiced Hand" && args.AchievementDesc == "Played 10 hands.");
            Assert.Contains(unlockedPopups, args => args.AchievementName == "Lil Big Score" && args.AchievementDesc.Contains("100"));
            Assert.Contains(unlockedPopups, args => args.AchievementName == "Brainstorm Unlocked" && args.AchievementDesc.Contains("royal flush"));
        }
        finally
        {
            EngineEventHandler.StopListening(listener);
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void PersistentJokerUnlockAchievements_TrackAcrossRunsAndPersist()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            EngineEventHandler.ResetFullEventHandler();
            UnlockManager.ResetProgressToDefaults();

            for (var i = 0; i < 4; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.RunLost } });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.MrBones_UnlockId));
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.RunLost } });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.MrBones_UnlockId));

            for (var i = 0; i < 199; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.Acrobat_UnlockId));
            EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Acrobat_UnlockId));

            for (var i = 0; i < 19; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineCardSoldArgs
                {
                    MyContext = new EventContext { Context = EventContextType.CardSell },
                    CardBeingSold = JokerDb.GenerateJokerCard("JIMBO"),
                });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.Swashbuckler_UnlockId));
            EngineEventHandler.TriggerEvent(new EngineCardSoldArgs
            {
                MyContext = new EventContext { Context = EventContextType.CardSell },
                CardBeingSold = JokerDb.GenerateJokerCard("JIMBO"),
            });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Swashbuckler_UnlockId));

            UnlockManager.ResetProgressToDefaults();
            Assert.True(UnlockManager.LoadProgress());
            Assert.Equal(5, UnlockManager.GetPersistentProgressCount(UnlockManager.LostRunsProgressKey));
            Assert.Equal(200, UnlockManager.GetPersistentProgressCount(UnlockManager.HandsPlayedProgressKey));
            Assert.Equal(20, UnlockManager.GetPersistentProgressCount(UnlockManager.JokersSoldProgressKey));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.MrBones_UnlockId));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Acrobat_UnlockId));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Swashbuckler_UnlockId));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void PersistentJokerUnlockAchievements_TrackFaceCardsAndCardsSold()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            EngineEventHandler.ResetFullEventHandler();
            UnlockManager.ResetProgressToDefaults();

            for (var i = 0; i < 99; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineHandPlayArgs
                {
                    MyContext = new EventContext { Context = EventContextType.CardsSelectedForPlay },
                    CardsSelected = CardFactory.CardListFromDefString("KS,QS,JS", ","),
                });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.SockAndBuskin_UnlockId));
            EngineEventHandler.TriggerEvent(new EngineHandPlayArgs
            {
                MyContext = new EventContext { Context = EventContextType.CardsSelectedForPlay },
                CardsSelected = CardFactory.CardListFromDefString("KH,QH,JH", ","),
            });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.SockAndBuskin_UnlockId));

            for (var i = 0; i < 49; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineCardSoldArgs
                {
                    MyContext = new EventContext { Context = EventContextType.CardSell },
                    CardBeingSold = CardFactory.CardListFromDefString("AS", ",").Single(),
                });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.BurntJoker_UnlockId));
            EngineEventHandler.TriggerEvent(new EngineCardSoldArgs
            {
                MyContext = new EventContext { Context = EventContextType.CardSell },
                CardBeingSold = CardFactory.CardListFromDefString("AH", ",").Single(),
            });
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.BurntJoker_UnlockId));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void Collection_TracksJokersAndConsumables_FromEventsAndPersists()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            EngineEventHandler.ResetFullEventHandler();
            ZoneManager.InitializeMainGameZones();
            UnlockManager.ResetProgressToDefaults();

            var jimbo = JokerDb.GenerateJokerCard("JIMBO");
            EngineEventHandler.TriggerEvent(new EngineCardDrawnToZoneArgs
            {
                MyContext = new EventContext { Context = EventContextType.CardDrawnToZone },
                CardBeingDrawn = jimbo,
                ZoneDrawnTo = ZoneManager.JokerZone,
            });

            EngineEventHandler.TriggerEvent(new EngineConsumableUseArgs
            {
                MyContext = new EventContext { Context = EventContextType.ConsumableUsed },
                ConsumableDBName = "FOOL",
            });

            var pluto = ConsumableManager.MakePlanetCard(PlayedHandType.HIGHCARD);
            EngineEventHandler.TriggerEvent(new EngineConsumableUseArgs
            {
                MyContext = new EventContext { Context = EventContextType.ConsumableUsed },
                ConsumableDBName = pluto.ConsumableData.DBName,
            });

            Assert.True(UnlockManager.IsJokerCollected("JIMBO"));
            Assert.True(UnlockManager.IsConsumableCollected("FOOL"));
            Assert.True(UnlockManager.IsConsumableCollected("PLUTO"));
            Assert.Equal(3, UnlockManager.CollectionCount);

            UnlockManager.ResetProgressToDefaults();
            Assert.False(UnlockManager.IsJokerCollected("JIMBO"));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(UnlockManager.IsJokerCollected("JIMBO"));
            Assert.True(UnlockManager.IsConsumableCollected("FOOL"));
            Assert.True(UnlockManager.IsConsumableCollected("PLUTO"));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void MoneyTreeAchievement_RequiresTenConsecutiveMaxInterestRounds()
    {
        EngineEventHandler.ResetFullEventHandler();
        UnlockManager.ResetProgressToDefaults();
        Globals.CurMaxInterest = 5;
        Globals.Money = 25;
        List<(string, int)> list = null;
        for (var i = 0; i < 9; i++)
        {
            list = FlowHandler.CalcPostRoundMoneyWithSources();
        }
        Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.MoneyTree_UnlockId));

        Globals.Money = 24;
        list = FlowHandler.CalcPostRoundMoneyWithSources();
        Globals.Money = 25;
        for (var i = 0; i < 9; i++)
        {
            list = FlowHandler.CalcPostRoundMoneyWithSources();
        }
        Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.MoneyTree_UnlockId));

        list = FlowHandler.CalcPostRoundMoneyWithSources();
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.MoneyTree_UnlockId));
    }

    [Fact]
    public void BossBlindCollection_PersistsAndUnlocksRetconAfterTwentyFiveDiscoveries()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            EngineEventHandler.ResetFullEventHandler();
            UnlockManager.ResetProgressToDefaults();

            var bossBlinds = BossBlindDb.BossBlindNames.Take(25).ToList();
            foreach (var bossBlind in bossBlinds.Take(24))
            {
                Assert.True(UnlockManager.AddBossBlindToCollection(bossBlind));
            }
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.Retcon_UnlockId));

            Assert.True(UnlockManager.AddBossBlindToCollection(bossBlinds[24]));
            Assert.Equal(25, UnlockManager.CollectedBossBlindCount);
            Assert.True(UnlockManager.IsBossBlindCollected(bossBlinds[0]));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.Retcon_UnlockId));

            UnlockManager.ResetProgressToDefaults();
            Assert.True(UnlockManager.LoadProgress());
            Assert.Equal(25, UnlockManager.CollectedBossBlindCount);
            Assert.True(UnlockManager.IsBossBlindCollected(bossBlinds[0]));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void PermanentProgressSavingDisabled_AllowsProgressButSkipsFileWrites()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            Assert.True(DeckDb.UnlockDeck("BLUE"));
            Assert.True(UnlockManager.MarkAchievementAchieved("DEBUG_ACHIEVEMENT"));
            Assert.True(UnlockManager.AddJokerToCollection("JIMBO"));

            Assert.True(DeckDb.IsDeckUnlocked("BLUE"));
            Assert.True(UnlockManager.IsAchievementAchieved("DEBUG_ACHIEVEMENT"));
            Assert.True(UnlockManager.IsJokerCollected("JIMBO"));
            Assert.False(File.Exists(savePath));
            Assert.False(File.Exists(savePath + ".tmp"));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }

    private static string BuildTempSavePath()
    {
        return Path.Combine(Path.GetTempPath(), "ConsoleBalatroTests", $"unlocks-{Guid.NewGuid():N}.json");
    }

    private static void DeleteTempSave(string savePath)
    {
        var directory = Path.GetDirectoryName(savePath);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        if (File.Exists(savePath + ".tmp"))
        {
            File.Delete(savePath + ".tmp");
        }
        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }
    }
}
