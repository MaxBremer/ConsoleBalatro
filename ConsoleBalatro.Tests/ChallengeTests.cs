using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Challenges;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools;
using ConsoleBalatro.Engine.Pools.Rollables;
using ConsoleBalatro.Engine.Pools.Rules;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Stakes;
using ConsoleBalatro.UI.EngineUI;
using ConsoleBalatro.UI.EngineUI.Controls;
using Xunit;

namespace ConsoleBalatro.Tests;

public class ChallengeTests : TestClassBase
{
    [Fact]
    public void Fragile_PackAndTagPoolsExcludeSourcesOfNonGlassCards()
    {
        StartChallenge("FRAGILE");

        var bannedPacks = new HashSet<string>
        {
            "BASIC CARD PACK",
            "JUMBO CARD PACK",
            "MEGA CARD PACK",
        };

        for (var i = 0; i < 100; i++)
        {
            var pack = PoolManager.RollSingle(new ContentRollRequest
            {
                Pool = ItemPool.Pack,
                Source = GenerationSource.Shop,
            });
            Assert.DoesNotContain(pack.Id, bannedPacks);

            var tag = Assert.IsType<TagRollableDefinition>(PoolManager.RollSingle(new ContentRollRequest
            {
                Pool = ItemPool.Tag,
                Source = GenerationSource.Tag,
            }));
            Assert.NotEqual(TagType.MEGA_STANDARD, tag.TagType);
        }
    }

    [Fact]
    public void Challenges_DefaultToNoneUnlocked_AndCannotToggleChallengeMode()
    {
        UnlockManager.ResetProgressToDefaults();

        Assert.Empty(ChallengeManager.UnlockedChallengeIds);
        var display = new DeckChoicesDisplay(0, 0);
        display.ToggleRunType();
        Assert.False(display.IsChallengeMode);
        display.PreDisplaySetup();
        Assert.DoesNotContain("Tab: challenges", GetDisplayRow(display, 3));
    }

    [Fact]
    public void DebugUnlockFirstFive_AllowsChallengeModeAndNormalProgression()
    {
        UnlockManager.ResetProgressToDefaults();

        DebugManager.ReadCommand("unlockchallenges 5");
        DebugManager.ReadCommand("unlockchallenges 5");

        Assert.Equal(5, ChallengeManager.UnlockedChallengeIds.Count);
        Assert.All(ChallengeManager.All, challenge => Assert.Equal(challenge.ChallengeIndex <= 5,
            ChallengeManager.IsUnlocked(challenge.Id)));
        var display = new DeckChoicesDisplay(0, 0);
        display.ToggleRunType();
        Assert.True(display.IsChallengeMode);
        Assert.Empty(UnlockManager.BeatenChallengeIds);
        Assert.Equal(0, UnlockManager.WonDeckCount);
        Assert.DoesNotContain(AchievementDb.AllChallengesCompleteId, UnlockManager.AchievedAchievementIds);

        UnlockManager.MarkChallengeBeaten("X RAY VISION", saveImmediately: false);
        Assert.Equal(6, ChallengeManager.UnlockedChallengeIds.Count);
        Assert.True(ChallengeManager.IsUnlocked("MAD WORLD"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DebugUnlockAll_RespectsPermanentProgressSetting(bool savingDisabled)
    {
        var savePath = Path.Combine(Path.GetTempPath(), $"console-balatro-challenges-{Guid.NewGuid():N}.json");
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = savingDisabled;
            UnlockManager.ResetProgressToDefaults();

            DebugManager.ReadCommand("unlockchallenges all");

            Assert.Equal(20, ChallengeManager.UnlockedChallengeIds.Count);
            Assert.All(AchievementDb.ChallengeUnlocksByAchievementId.Keys,
                achievementId => Assert.True(UnlockManager.IsAchievementAchieved(achievementId)));
            Assert.Empty(UnlockManager.BeatenChallengeIds);
            Assert.Equal(0, UnlockManager.WonDeckCount);
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.AllChallengesCompleteId));
            Assert.Equal(!savingDisabled, File.Exists(savePath));
            Assert.False(File.Exists(savePath + ".tmp"));

            if (!savingDisabled)
            {
                UnlockManager.ResetProgressToDefaults();
                Assert.True(UnlockManager.LoadProgress());
                Assert.Equal(20, ChallengeManager.UnlockedChallengeIds.Count);
                var display = new DeckChoicesDisplay(0, 0);
                display.ToggleRunType();
                Assert.True(display.IsChallengeMode);
                Assert.Empty(UnlockManager.BeatenChallengeIds);
                Assert.Equal(0, UnlockManager.WonDeckCount);
                Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.AllChallengesCompleteId));
            }
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults();
            File.Delete(savePath);
        }
    }

    [Theory]
    [InlineData("unlockchallenges")]
    [InlineData("unlockchallenges 0")]
    [InlineData("unlockchallenges 6")]
    [InlineData("unlockchallenges 5 all")]
    [InlineData("unlockchallenges all -beaten")]
    public void DebugUnlockChallenges_InvalidArgumentsLeaveProgressUnchanged(string command)
    {
        UnlockManager.ResetProgressToDefaults();

        DebugManager.ReadCommand(command);

        Assert.Empty(ChallengeManager.UnlockedChallengeIds);
        Assert.Empty(UnlockManager.AchievedAchievementIds);
        Assert.Empty(UnlockManager.BeatenChallengeIds);
        Assert.Equal(0, UnlockManager.WonDeckCount);
    }

    [Fact]
    public void WinningFiveDifferentDecks_UnlocksFirstFiveChallengesAndMode()
    {
        UnlockManager.ResetProgressToDefaults();
        var display = new DeckChoicesDisplay(0, 0);
        foreach (var deck in new[] { "RED", "BLUE", "YELLOW", "GREEN" })
            UnlockManager.MarkDeckStakeBeaten(deck, StakeType.WHITE, saveImmediately: false);

        // Replays and higher stakes on the same deck do not count as another deck.
        Assert.False(UnlockManager.MarkDeckStakeBeaten("RED", StakeType.WHITE, saveImmediately: false));
        UnlockManager.MarkDeckStakeBeaten("RED", StakeType.RED, saveImmediately: false);
        Assert.Equal(4, UnlockManager.WonDeckCount);
        Assert.Empty(ChallengeManager.UnlockedChallengeIds);
        display.ToggleRunType();
        Assert.False(display.IsChallengeMode);

        UnlockManager.MarkDeckStakeBeaten("BLACK", StakeType.WHITE, saveImmediately: false);

        Assert.Equal(5, UnlockManager.WonDeckCount);
        Assert.Equal(5, ChallengeManager.UnlockedChallengeIds.Count);
        Assert.All(ChallengeManager.All, challenge => Assert.Equal(challenge.ChallengeIndex <= 5, ChallengeManager.IsUnlocked(challenge.Id)));
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeUnlockId));
        display.PreDisplaySetup();
        Assert.Contains("Tab: challenges", GetDisplayRow(display, 3));
        display.ToggleRunType();
        Assert.True(display.IsChallengeMode);
        display.ToggleRunType();
        Assert.False(display.IsChallengeMode);
    }

    [Fact]
    public void DistinctChallengeWins_UnlockNextChallenge_RegardlessOfOrder()
    {
        UnlockManager.ResetProgressToDefaults();
        UnlockChallengeMode();

        for (var completed = 1; completed <= 20; completed++)
        {
            // Always beat the highest available challenge, leaving the earliest ones until last.
            var next = ChallengeManager.All.Where(challenge => ChallengeManager.IsUnlocked(challenge.Id)
                && !ChallengeManager.IsBeaten(challenge.Id)).MaxBy(challenge => challenge.ChallengeIndex)!;
            Assert.True(UnlockManager.MarkChallengeBeaten(next.Id, saveImmediately: false));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.ChallengeCompletionAchievementIds[next.Id]));
            Assert.Equal(completed, UnlockManager.BeatenChallengeIds.Count);
            Assert.Equal(Math.Min(20, 5 + completed), ChallengeManager.UnlockedChallengeIds.Count);
            Assert.All(ChallengeManager.All, challenge => Assert.Equal(challenge.ChallengeIndex <= 5 + completed,
                ChallengeManager.IsUnlocked(challenge.Id)));
            Assert.False(UnlockManager.MarkChallengeBeaten(next.Id, saveImmediately: false));
            Assert.Equal(Math.Min(20, 5 + completed), ChallengeManager.UnlockedChallengeIds.Count);
            Assert.Equal(completed == 20, UnlockManager.IsAchievementAchieved(AchievementDb.AllChallengesCompleteId));
        }

        Assert.DoesNotContain(AchievementDb.AllChallengesCompleteId, AchievementDb.ChallengeUnlocksByAchievementId.Keys);
    }

    [Fact]
    public void OnlyAnteEightBossWins_CountTowardChallengeMode()
    {
        ResetEngineForTest();
        UnlockManager.ResetProgressToDefaults();
        foreach (var deck in new[] { "RED", "BLUE", "YELLOW", "GREEN", "BLACK" })
        {
            FlowHandler.CurrentDeckDbName = deck;
            FlowHandler.CurrentAnte = 7;
            FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
            FlowHandler.IncrementBlind();
        }
        Assert.Equal(0, UnlockManager.WonDeckCount);
        Assert.Empty(ChallengeManager.UnlockedChallengeIds);

        foreach (var deck in new[] { "RED", "BLUE", "YELLOW", "GREEN", "BLACK" })
        {
            FlowHandler.CurrentDeckDbName = deck;
            FlowHandler.CurrentAnte = 8;
            FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
            FlowHandler.IncrementBlind();
        }
        Assert.Equal(5, UnlockManager.WonDeckCount);
        Assert.Equal(5, ChallengeManager.UnlockedChallengeIds.Count);
    }

    [Fact]
    public void ExistingSaveProgress_AwardsNewChallengeAchievementsOnLoad()
    {
        var savePath = Path.Combine(Path.GetTempPath(), $"console-balatro-challenges-{Guid.NewGuid():N}.json");
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.ResetProgressToDefaults();
            File.WriteAllText(savePath, """
                {
                  "Version": 2,
                  "Decks": { "HighestBeatenStakeIndexes": { "RED": 0, "BLUE": 0, "YELLOW": 0, "GREEN": 0, "BLACK": 0 } },
                  "Challenges": { "Beaten": ["THE OMELETTE", "15 MINUTE CITY", "X RAY VISION"] }
                }
                """);

            Assert.True(UnlockManager.LoadProgress());
            Assert.Equal(8, ChallengeManager.UnlockedChallengeIds.Count);
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeUnlockId));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.ChallengeCompletionAchievementIds["X RAY VISION"]));
            Assert.False(UnlockManager.IsAchievementAchieved(AchievementDb.AllChallengesCompleteId));
        }
        finally
        {
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults();
            File.Delete(savePath);
        }
    }

    [Fact]
    public void ChallengeProgress_PersistsWithUnlocksAndCompletionAchievement()
    {
        var savePath = Path.Combine(Path.GetTempPath(), $"console-balatro-challenges-{Guid.NewGuid():N}.json");
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.PermanentProgressSavingDisabled = false;
            UnlockManager.ResetProgressToDefaults();
            UnlockChallengeMode();

            UnlockManager.MarkChallengeBeaten("THE OMELETTE", saveImmediately: false);
            UnlockManager.MarkChallengeBeaten("15 MINUTE CITY");
            Assert.True(File.Exists(savePath));

            UnlockManager.ResetProgressToDefaults();
            Assert.False(ChallengeManager.IsBeaten("THE OMELETTE"));
            Assert.False(ChallengeManager.IsUnlocked("RICH GET RICHER"));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(ChallengeManager.IsBeaten("THE OMELETTE"));
            Assert.True(ChallengeManager.IsBeaten("15 MINUTE CITY"));
            Assert.True(ChallengeManager.IsUnlocked("RICH GET RICHER"));
            Assert.Equal(7, ChallengeManager.UnlockedChallengeIds.Count);
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeUnlockId));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeWinId));
            Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.FifteenMinuteCityChallengeWinId));
        }
        finally
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults();
            if (File.Exists(savePath))
                File.Delete(savePath);
        }
    }

    [Fact]
    public void LockedChallenge_CannotBeStarted()
    {
        ResetEngineForTest();
        UnlockManager.ResetProgressToDefaults();

        FlowHandler.ChallengeChosen("RICH GET RICHER");

        Assert.Null(ChallengeManager.CurrentChallenge);
    }

    [Fact]
    public void WinningAnteEightBoss_MarksCurrentChallengeBeaten()
    {
        ResetEngineForTest();
        UnlockManager.ResetProgressToDefaults();
        ChallengeManager.Unlock("THE OMELETTE", saveImmediately: false);
        FlowHandler.ChallengeChosen("THE OMELETTE");
        FlowHandler.CurrentAnte = 8;
        FlowHandler.CurrentSelectedBlind = BlindType.BOSS;

        FlowHandler.IncrementBlind();

        Assert.True(ChallengeManager.IsBeaten("THE OMELETTE"));
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeWinId));
        Assert.Equal(0, UnlockManager.WonDeckCount);
    }

    [Fact]
    public void Omelette_StartsWithFiveEggs()
    {
        ResetEngineForTest();
        ChallengeManager.Unlock("THE OMELETTE", saveImmediately: false);

        FlowHandler.ChallengeChosen("THE OMELETTE");

        Assert.Equal("The Omelette", ChallengeManager.CurrentChallenge?.Name);
        Assert.Equal(5, ZoneManager.JokerZone?.Cards.Count);
        Assert.All(ZoneManager.JokerZone!.Cards,
            card => Assert.Equal("EGG", card.JokerData?.DBName));
    }

    [Fact]
    public void FifteenMinuteCity_HasCorrectStartingJokersAndDeck()
    {
        StartChallenge("15 MINUTE CITY");

        var jokers = ZoneManager.JokerZone!.Cards;

        Assert.Equal(2, jokers.Count);

        Assert.Contains(jokers, j =>
            j.JokerData?.DBName == "RIDE THE BUS"
            && j.HasSticker(Sticker.ETERNAL));

        Assert.Contains(jokers, j =>
            j.JokerData?.DBName == "SHORTCUT"
            && j.HasSticker(Sticker.ETERNAL));

        var deck = ZoneManager.DeckZone!.Cards;

        Assert.DoesNotContain(deck, c =>
            c.Rank is Rank.ACE or Rank.TWO or Rank.THREE);

        // There should be two copies of every face card:
        // 3 ranks * 4 suits * 2 copies = 24.
        Assert.Equal(24, deck.Count(c =>
            c.Rank is Rank.JACK or Rank.QUEEN or Rank.KING));

        Assert.Equal(52, deck.Count);
    }

    [Fact]
    public void BramPoker_RemovesJokersFromShopRolls()
    {
        ResetEngineForTest();

        Assert.True(ChallengeManager.TryGet("BRAM POKER", out ChallengeDefinition bramChal));
        ChallengeManager.Begin(bramChal);

        var args = new EngineMarketTypeBeingChosenArgs
        {
            WeightsBeingRolled = MarketPullManager.MainMarketWeights.ToDictionary(),
            MyContext = new()
            {
                Context = EventContextType.MarketTypeBeingChosen
            }
        };

        EngineEventHandler.TriggerEvent(args);

        Assert.DoesNotContain(BuyItemType.JOKER, args.WeightsBeingRolled.Keys);
    }

    [Fact]
    public void RichGetRicher_StartsWithOneHundredDollarsAndMoneyVouchers()
    {
        StartChallenge("RICH GET RICHER");

        Assert.Equal(100, Globals.Money);

        var vouchers = ZoneManager.ActiveVoucherZone!.Cards;

        Assert.Contains(vouchers,
            c => c.JokerData?.DBName == "SEED MONEY");

        Assert.Contains(vouchers,
            c => c.JokerData?.DBName == "MONEY TREE");
    }

    [Fact]
    public void RichGetRicher_ChipsCannotExceedCurrentMoney()
    {
        StartChallenge("RICH GET RICHER");

        Globals.CurrentChips = 90;

        Globals.EmitChipsAdd(25, new Card());

        Assert.Equal(100, Globals.CurrentChips);

        // Once we're already at the money cap, further chip gain does nothing.
        Globals.EmitChipsAdd(20, new Card());

        Assert.Equal(100, Globals.CurrentChips);
    }

    [Fact]
    public void OnAKnifesEdge_StartsWithPinnedEternalCeremonialDagger()
    {
        StartChallenge("ON A KNIFES EDGE");

        var dagger = Assert.Single(ZoneManager.JokerZone!.Cards);

        Assert.Equal("CEREMONIAL DAGGER", dagger.JokerData?.DBName);
        Assert.True(dagger.HasSticker(Sticker.ETERNAL));
        Assert.True(dagger.Pinned);
    }

    [Fact]
    public void XRayVision_SuccessfulOneInFourRollTurnsDrawnCardFaceDown()
    {
        StartChallenge("X RAY VISION");

        var card = CardFactory.PlayingCardFromRankSuit(
            Rank.ACE,
            Suit.SPADES);

        Assert.False(card.FaceDown);

        RigNextRoll(true);

        EngineEventHandler.TriggerEvent(
            new EngineCardDrawnToZoneArgs
            {
                MyContext = new()
                {
                    Context = EventContextType.CardDrawnToZone
                },
                CardBeingDrawn = card,
                ZoneDrawnTo = ZoneManager.HandZone
            });

        Assert.True(card.FaceDown);
    }

    [Fact]
    public void XRayVision_FailedOneInFourRollLeavesDrawnCardFaceUp()
    {
        StartChallenge("X RAY VISION");

        var card = CardFactory.PlayingCardFromRankSuit(
            Rank.ACE,
            Suit.SPADES);

        RigNextRoll(false);

        EngineEventHandler.TriggerEvent(
            new EngineCardDrawnToZoneArgs
            {
                MyContext = new()
                {
                    Context = EventContextType.CardDrawnToZone
                },
                CardBeingDrawn = card,
                ZoneDrawnTo = ZoneManager.HandZone
            });

        Assert.False(card.FaceDown);
    }

    [Fact]
    public void MadWorld_HasCorrectStartingJokersAndDeck()
    {
        var definition = StartChallenge("MAD WORLD");

        var jokers = ZoneManager.JokerZone!.Cards;

        var pareidolia = Assert.Single(jokers, j => j.JokerData?.DBName == "PAREIDOLIA");

        Assert.True(pareidolia.HasSticker(Sticker.ETERNAL));
        Assert.Equal(Edition.NEGATIVE, pareidolia.Edition);

        var businessCard = Assert.Single(jokers, j => j.JokerData?.DBName == "BUSINESS CARD");

        Assert.True(businessCard.HasSticker(Sticker.ETERNAL));

        Assert.All(
            ZoneManager.DeckZone!.Cards,
            card => Assert.Contains(
                card.Rank,
                new[]
                {
                Rank.TWO,
                Rank.THREE,
                Rank.FOUR,
                Rank.FIVE,
                Rank.SIX,
                Rank.SEVEN,
                Rank.EIGHT,
                Rank.NINE
                }));

        Assert.Contains("THE PLANT", definition.BannedBossBlinds);
        Assert.Equal(32, ZoneManager.DeckZone.Cards.Count);
    }

    [Fact]
    public void MadWorld_RemovesInterestAndUnusedHandMoney()
    {
        StartChallenge("MAD WORLD");

        var args = new EngineGatherPostRoundMoneyArgs
        {
            MyContext = new()
            {
                Context = EventContextType.GatherPostRoundMoney
            }
        };

        args.ExistingSources.Add(("Interest", 5));
        args.ExistingSources.Add(("Hands Remaining", 3));
        args.ExistingSources.Add(("Blind", 4));

        EngineEventHandler.TriggerEvent(args);

        Assert.DoesNotContain(
            args.ExistingSources,
            x => x.Item1 == "Interest");

        Assert.DoesNotContain(
            args.ExistingSources,
            x => x.Item1 == "Hands Remaining");

        // Unlike Omelette, blind reward should remain.
        Assert.Contains(
            args.ExistingSources,
            x => x.Item1 == "Blind");
    }

    [Fact]
    public void LuxuryTax_HandSizeChangesWithMoney()
    {
        StartChallenge("LUXURY TAX");

        Assert.Equal(4, Globals.Money);
        Assert.Equal(10, Globals.HandSize);

        // $9 total -> floor(9 / 5) = 1 card reduction.
        Globals.EmitMoneyGain(5, null);

        Assert.Equal(9, Globals.Money);
        Assert.Equal(9, Globals.HandSize);

        // $14 total -> floor(14 / 5) = 2 card reduction.
        Globals.EmitMoneyGain(5, null);

        Assert.Equal(14, Globals.Money);
        Assert.Equal(8, Globals.HandSize);

        // Losing money should restore hand size as well.
        Globals.EmitMoneyGain(-10, null);

        Assert.Equal(4, Globals.Money);
        Assert.Equal(10, Globals.HandSize);
    }

    [Fact]
    public void NonPerishable_GeneratedJokersBecomeEternal()
    {
        StartChallenge("NON-PERISHABLE");

        var joker = MarketPullManager.PickMarketCard(
            BuyItemType.JOKER,
            GenerationSource.Shop);

        Assert.True(joker.IsJoker);
        Assert.True(joker.HasSticker(Sticker.ETERNAL));
    }

    [Fact]
    public void NonPerishable_BansDestructibleJokersAndVerdantLeaf()
    {
        var definition = StartChallenge("NON-PERISHABLE");

        var banned = definition.BannedPoolItems[ItemPool.Joker];

        Assert.Contains("GROS MICHEL", banned);
        Assert.Contains("CAVENDISH", banned);
        Assert.Contains("ICE CREAM", banned);
        Assert.Contains("TURTLE BEAN", banned);
        Assert.Contains("RAMEN", banned);
        Assert.Contains("DIET COLA", banned);
        Assert.Contains("SELTZER", banned);
        Assert.Contains("POPCORN", banned);
        Assert.Contains("MR. BONES", banned);
        Assert.Contains("INVISIBLE JOKER", banned);
        Assert.Contains("LUCHADOR", banned);

        Assert.Contains("VERDANT LEAF", definition.BannedBossBlinds);
    }

    [Fact]
    public void Medusa_StartsWithEternalMarbleJokerAndStoneFaceCards()
    {
        StartChallenge("MEDUSA");

        var marble = Assert.Single(ZoneManager.JokerZone!.Cards);

        Assert.Equal("MARBLE JOKER", marble.JokerData?.DBName);
        Assert.True(marble.HasSticker(Sticker.ETERNAL));

        var deck = ZoneManager.DeckZone!.Cards;

        // J, Q, K in four suits = 12 Stone cards.
        Assert.Equal(
            12,
            deck.Count(c => c.Enhancement == Enhancement.STONE));

        Assert.Equal(52, deck.Count);
    }

    [Fact]
    public void DoubleOrNothing_AllStartingCardsHaveRedSeals()
    {
        StartChallenge("DOUBLE OR NOTHING");

        Assert.All(
            ZoneManager.DeckZone!.Cards,
            card => Assert.Equal(Seal.RED, card.Seal));
    }

    [Fact]
    public void DoubleOrNothing_PlayingCardBecomesDebuffedAfterScoring()
    {
        StartChallenge("DOUBLE OR NOTHING");

        var card = CardFactory.PlayingCardFromRankSuit(
            Rank.ACE,
            Suit.SPADES);

        Assert.False(card.Debuffed);

        EngineEventHandler.TriggerEvent(
            new EngineCardTriggerArgs
            {
                MyContext = new()
                {
                    Context = EventContextType.CardTrigger
                },
                CardThatIsTriggering = card,
                isPostScoringTrigger = true
            });

        Assert.True(card.Debuffed);
    }

    [Fact]
    public void Typecast_AtAnteFiveMakesJokersEternalAndSetsSlotsToZero()
    {
        var definition = StartChallenge("TYPECAST");

        AddJoker("JIMBO");
        AddJoker("GREEDY JOKER");

        Assert.All(
            ZoneManager.JokerZone!.Cards,
            joker => Assert.False(joker.HasSticker(Sticker.ETERNAL)));

        Assert.True(ZoneManager.JokerZone.MaxCapacity > 0);

        FlowHandler.CurrentAnte = 5;

        Assert.All(
            ZoneManager.JokerZone.Cards,
            joker => Assert.True(joker.HasSticker(Sticker.ETERNAL)));

        Assert.Equal(0, ZoneManager.JokerZone.MaxCapacity);

        Assert.Contains(
            "VERDANT LEAF",
            definition.BannedBossBlinds);
    }

    [Fact]
    public void Typecast_DoesNotTriggerBeforeAnteFive()
    {
        StartChallenge("TYPECAST");

        AddJoker("GREEDY JOKER");

        FlowHandler.CurrentAnte = 4;

        Assert.False(
            ZoneManager.JokerZone!.Cards.Single()
                .HasSticker(Sticker.ETERNAL));

        Assert.True(ZoneManager.JokerZone.MaxCapacity > 0);
    }

    [Fact]
    public void Inflation_StartsWithCreditCardAndBansDiscountVouchers()
    {
        var definition = StartChallenge("INFLATION");

        Assert.Contains(
            ZoneManager.JokerZone!.Cards,
            c => c.JokerData?.DBName == "CREDIT CARD");

        Assert.Contains(
            "CLEARANCE SALE",
            definition.BannedPoolItems[ItemPool.Voucher]);

        Assert.Contains(
            "LIQUIDATION",
            definition.BannedPoolItems[ItemPool.Voucher]);
    }

    [Fact]
    public void Inflation_EachPurchasePermanentlyRaisesPricesByOne()
    {
        StartChallenge("INFLATION");

        var card = CardFactory.PlayingCardFromRankSuit(
            Rank.ACE,
            Suit.SPADES);

        var originalPrice = card.BuyCost;

        EngineEventHandler.TriggerEvent(
            new EngineCardPurchasedArgs
            {
                BeingPurchased = card,
                AmountPaid = originalPrice
            });

        Assert.Equal(originalPrice + 1, card.BuyCost);

        EngineEventHandler.TriggerEvent(
            new EngineCardPurchasedArgs
            {
                BeingPurchased = card,
                AmountPaid = card.BuyCost
            });

        Assert.Equal(originalPrice + 2, card.BuyCost);
    }

    [Fact]
    public void BramPoker_HasCorrectStartingCardsAndVouchers()
    {
        StartChallenge("BRAM POKER");

        var vampire = Assert.Single(ZoneManager.JokerZone!.Cards);
        Assert.Equal("VAMPIRE", vampire.JokerData?.DBName);
        Assert.True(vampire.HasSticker(Sticker.ETERNAL));
        Assert.Equal(new[] { "EMPEROR", "EMPRESS" },
            ZoneManager.ConsumableZone!.Cards.Select(c => c.ConsumableData!.DBName));
        Assert.Equal(new[] { "MAGIC TRICK", "ILLUSION" },
            ZoneManager.ActiveVoucherZone!.Cards.Select(c => c.JokerData!.DBName));
        Assert.True(Globals.ShopPlayingCardsGetModifiers);
    }

    [Fact]
    public void Fragile_StartsWithGlassDeckAndTwoEternalNegativeOopsAllSixes()
    {
        StartChallenge("FRAGILE");

        Assert.Equal(52, ZoneManager.DeckZone!.Cards.Count);
        Assert.All(ZoneManager.DeckZone.Cards,
            card => Assert.Equal(Enhancement.GLASS, card.Enhancement));
        Assert.Equal(2, ZoneManager.JokerZone!.Cards.Count);
        Assert.All(ZoneManager.JokerZone.Cards, joker =>
        {
            Assert.Equal("OOPS! ALL 6S", joker.JokerData?.DBName);
            Assert.True(joker.HasSticker(Sticker.ETERNAL));
            Assert.Equal(Edition.NEGATIVE, joker.Edition);
        });
        Assert.Equal(7, ZoneManager.JokerZone.MaxCapacity);
    }

    [Fact]
    public void Monolith_StartsWithEternalObeliskAndEternalNegativeMarbleJoker()
    {
        StartChallenge("MONOLITH");

        var jokers = ZoneManager.JokerZone!.Cards;
        Assert.Equal(2, jokers.Count);
        var obelisk = Assert.Single(jokers, j => j.JokerData?.DBName == "OBELISK");
        Assert.True(obelisk.HasSticker(Sticker.ETERNAL));
        Assert.Equal(Edition.BASE, obelisk.Edition);
        var marble = Assert.Single(jokers, j => j.JokerData?.DBName == "MARBLE JOKER");
        Assert.True(marble.HasSticker(Sticker.ETERNAL));
        Assert.Equal(Edition.NEGATIVE, marble.Edition);
        Assert.Equal(6, ZoneManager.JokerZone.MaxCapacity);
    }

    [Fact]
    public void BlastOff_StartsWithEternalConstellationAndRocketAndReducedLimits()
    {
        StartChallenge("BLAST OFF");

        Assert.Equal(new[] { "CONSTELLATION", "ROCKET" },
            ZoneManager.JokerZone!.Cards.Select(c => c.JokerData!.DBName));
        Assert.All(ZoneManager.JokerZone.Cards,
            joker => Assert.True(joker.HasSticker(Sticker.ETERNAL)));
        Assert.Equal(4, ZoneManager.JokerZone.MaxCapacity);
        Assert.Equal(2, Globals.MaxHandsPerRound);
        Assert.Equal(2, Globals.MaxDiscardsPerRound);

        FlowHandler.StartSelectedBlind();

        Assert.Equal(2, Globals.CurHandsRemaining);
        Assert.Equal(2, Globals.CurDiscardsRemaining);
    }

    [Fact]
    public void BlastOff_StartsWithActivePlanetMerchantAndPlanetTycoon()
    {
        StartChallenge("BLAST OFF");

        Assert.Equal(new[] { "PLANET MERCHANT", "PLANET TYCOON" },
            ZoneManager.ActiveVoucherZone!.Cards.Select(c => c.JokerData!.DBName));

        var args = new EngineMarketTypeBeingChosenArgs
        {
            WeightsBeingRolled = MarketPullManager.MainMarketWeights.ToDictionary(),
            MyContext = new() { Context = EventContextType.MarketTypeBeingChosen }
        };
        var originalPlanetWeight = args.WeightsBeingRolled[BuyItemType.PLANET_CARD];

        EngineEventHandler.TriggerEvent(args);

        Assert.Equal(originalPlanetWeight * 4, args.WeightsBeingRolled[BuyItemType.PLANET_CARD]);
    }

    [Fact]
    public void FiveCardDraw_StartsWithCardSharpAndJimboAndCorrectRoundLimits()
    {
        StartChallenge("FIVE CARD DRAW");

        Assert.Equal(new[] { "CARD SHARP", "JIMBO" },
            ZoneManager.JokerZone!.Cards.Select(c => c.JokerData!.DBName));
        Assert.All(ZoneManager.JokerZone.Cards,
            joker => Assert.False(joker.HasSticker(Sticker.ETERNAL)));
        Assert.Equal(7, ZoneManager.JokerZone.MaxCapacity);
        Assert.Equal(5, Globals.HandSize);
        Assert.Equal(6, Globals.MaxDiscardsPerRound);
        Assert.Equal(4, Globals.MaxHandsPerRound);

        FlowHandler.StartSelectedBlind();

        Assert.Equal(5, ZoneManager.HandZone!.Cards.Count);
        Assert.Equal(6, Globals.CurDiscardsRemaining);
        Assert.Equal(4, Globals.CurHandsRemaining);
    }

    [Fact]
    public void GoldenNeedle_StartsWithCreditCardTenDollarsAndCorrectRoundLimits()
    {
        StartChallenge("GOLDEN NEEDLE");

        var creditCard = Assert.Single(ZoneManager.JokerZone!.Cards);
        Assert.Equal("CREDIT CARD", creditCard.JokerData?.DBName);
        Assert.Equal(10, Globals.Money);
        Assert.Equal(-20, Globals.MinimumMoneyAllowed);
        Assert.Equal(1, Globals.MaxHandsPerRound);
        Assert.Equal(6, Globals.MaxDiscardsPerRound);

        FlowHandler.StartSelectedBlind();

        Assert.Equal(1, Globals.CurHandsRemaining);
        Assert.Equal(6, Globals.CurDiscardsRemaining);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(0, 1, true)]
    [InlineData(-19, 1, true)]
    [InlineData(-20, 1, false)]
    [InlineData(10, 0, false)]
    public void GoldenNeedle_DiscardAvailabilityRespectsCreditLimitAndRemainingDiscards(
        int money, int discards, bool expected)
    {
        StartChallenge("GOLDEN NEEDLE");
        Globals.Money = money;
        Globals.CurDiscardsRemaining = discards;

        Assert.Equal(expected, Globals.CanDiscard);
        Assert.Equal(money, Globals.Money);
        Assert.Equal(discards, Globals.CurDiscardsRemaining);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(-19)]
    public void GoldenNeedle_DiscardCostsOneDollarForTheWholeHand(int money)
    {
        StartChallenge("GOLDEN NEEDLE");
        Globals.Money = money;
        Globals.CurDiscardsRemaining = 6;
        var cards = BuildKnownHand("AS,KH,QD");

        Globals.DiscardSelectedFromHand(doRedraw: false);

        Assert.Equal(money - 1, Globals.Money);
        Assert.Equal(5, Globals.CurDiscardsRemaining);
        Assert.Empty(ZoneManager.HandZone!.Cards);
        Assert.All(cards, card => Assert.Contains(card, ZoneManager.DiscardZone!.Cards));
        var discard = Assert.IsType<EngineDiscardDoneArgs>(
            EngineEventHandler.LastSavedOfType(EventContextType.HandDiscardDone));
        Assert.Equal(cards, discard.BeingDiscarded);
    }

    [Theory]
    [InlineData(-20, 6)]
    [InlineData(10, 0)]
    public void GoldenNeedle_UnavailableDiscardDoesNotMoveCardsChargeMoneyOrEmitEvent(int money, int discards)
    {
        StartChallenge("GOLDEN NEEDLE");
        Globals.Money = money;
        Globals.CurDiscardsRemaining = discards;
        var cards = BuildKnownHand("AS,KH");
        Assert.False(Globals.CanDiscard);

        Globals.DiscardSelectedFromHand(doRedraw: false);

        Assert.Equal(money, Globals.Money);
        Assert.Equal(discards, Globals.CurDiscardsRemaining);
        Assert.Equal(cards, ZoneManager.HandZone!.Cards);
        Assert.Empty(ZoneManager.DiscardZone!.Cards);
        Assert.Equal(0, EngineEventHandler.CountOfSaved(EventContextType.HandDiscardDone));
    }

    [Fact]
    public void Cruelty_StartsWithThreeJokerSlots()
    {
        StartChallenge("CRUELTY");

        Assert.Empty(ZoneManager.JokerZone!.Cards);
        Assert.Equal(3, ZoneManager.JokerZone.MaxCapacity);
    }

    [Theory]
    [InlineData(BlindType.SMALL, false)]
    [InlineData(BlindType.BIG, false)]
    [InlineData(BlindType.BOSS, true)]
    public void Cruelty_OnlyRemovesSmallAndBigBlindRewards(BlindType blind, bool keepsBlindReward)
    {
        StartChallenge("CRUELTY");
        FlowHandler.CurrentSelectedBlind = blind;
        Globals.Money = 10;
        Globals.CurHandsRemaining = 2;

        var rewards = FlowHandler.CalcPostRoundMoneyWithSources();

        Assert.Equal(keepsBlindReward, rewards.Any(reward => reward.Item1 == "Blind"));
        if (keepsBlindReward)
            Assert.Contains(("Blind", FlowHandler.PostRoundFreeMoney[blind]), rewards);
        Assert.Contains(("Interest", 2), rewards);
        Assert.Contains(("Hands Remaining", 2), rewards);
    }

    [Fact]
    public void Jokerless_StartsWithoutJokersAndRejectsOrdinaryJokers()
    {
        StartChallenge("JOKERLESS");

        Assert.Empty(ZoneManager.JokerZone!.Cards);
        Assert.Equal(0, ZoneManager.JokerZone.MaxCapacity);
        AddJoker("JIMBO");
        Assert.Empty(ZoneManager.JokerZone.Cards);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Jokerless_RemovesOnlyJokerShopWeightAndHandlesRepeatedEvents(bool includeJokers)
    {
        StartChallenge("JOKERLESS");
        var weights = MarketPullManager.MainMarketWeights.ToDictionary();
        weights.Remove(BuyItemType.JOKER);
        var expected = weights.ToDictionary();
        if (includeJokers)
            weights[BuyItemType.JOKER] = 40;
        var args = new EngineMarketTypeBeingChosenArgs
        {
            WeightsBeingRolled = weights,
            MyContext = new() { Context = EventContextType.MarketTypeBeingChosen }
        };

        EngineEventHandler.TriggerEvent(args);
        EngineEventHandler.TriggerEvent(args);

        Assert.Equal(expected.Count, weights.Count);
        Assert.All(expected, pair => Assert.Equal(pair.Value, weights[pair.Key]));
        Assert.DoesNotContain(BuyItemType.JOKER, weights.Keys);
    }

    [Theory]
    [InlineData("BLAST OFF", ItemPool.Joker, "BURGLAR", "JIMBO")]
    [InlineData("BLAST OFF", ItemPool.Voucher, "GRABBER", "SEED MONEY")]
    [InlineData("BLAST OFF", ItemPool.Voucher, "NACHO TONG", "SEED MONEY")]
    [InlineData("FIVE CARD DRAW", ItemPool.Joker, "JUGGLER", "JIMBO")]
    [InlineData("FIVE CARD DRAW", ItemPool.Joker, "TROUBADOUR", "JIMBO")]
    [InlineData("FIVE CARD DRAW", ItemPool.Joker, "TURTLE BEAN", "JIMBO")]
    [InlineData("GOLDEN NEEDLE", ItemPool.Joker, "BURGLAR", "JIMBO")]
    [InlineData("GOLDEN NEEDLE", ItemPool.Voucher, "GRABBER", "SEED MONEY")]
    [InlineData("GOLDEN NEEDLE", ItemPool.Voucher, "NACHO TONG", "SEED MONEY")]
    public void ChallengeRestrictions_RemoveBannedItemsAndKeepAllowedItems(
        string challenge, ItemPool pool, string bannedId, string allowedId)
    {
        StartChallenge(challenge);
        var definitions = pool == ItemPool.Joker ? PoolManager.JokerPool : PoolManager.VoucherPool;
        var context = new MarketPoolContext { Pool = pool, Source = GenerationSource.Shop };
        context.Candidates.Add(new WeightedCandidate { Definition = definitions[bannedId], Weight = 1 });
        context.Candidates.Add(new WeightedCandidate { Definition = definitions[allowedId], Weight = 2 });

        new ChallengePoolRule().ModifyCandidates(context);

        var remaining = Assert.Single(context.Candidates);
        Assert.Equal(allowedId, remaining.Definition.Id);
        Assert.Equal(2, remaining.Weight);
    }

    [Fact]
    public void ChallengePoolRule_RemovesConfiguredItems()
    {
        ResetEngineForTest();
        var definition = new ChallengeDefinition
        {
            Id = "TEST_RESTRICTION",
            Name = "Test restriction",
            ChallengeIndex = 0,
            Description = "Test only"
        };
        definition.BannedPoolItems[ItemPool.Joker] = new(StringComparer.OrdinalIgnoreCase) { "EGG" };
        ChallengeManager.Begin(definition);
        var context = new MarketPoolContext { Pool = ItemPool.Joker, Source = GenerationSource.Shop };
        context.Candidates.Add(new WeightedCandidate
        {
            Definition = PoolManager.JokerPool["EGG"],
            Weight = 1
        });

        new ChallengePoolRule().ModifyCandidates(context);

        Assert.Empty(context.Candidates);
    }

    private static void UnlockChallengeMode()
    {
        foreach (var deck in new[] { "RED", "BLUE", "YELLOW", "GREEN", "BLACK" })
            UnlockManager.MarkDeckStakeBeaten(deck, StakeType.WHITE, saveImmediately: false);
    }

    private static string GetDisplayRow(DeckChoicesDisplay display, int row) =>
        string.Concat(Enumerable.Range(0, display.Width).Select(column => display.Sprite[row, column]));

    private ChallengeDefinition StartChallenge(string id)
    {
        ResetEngineForTest();

        var definition = ChallengeManager.ChallengeDb[id]();
        ChallengeManager.Begin(definition);

        return definition;
    }
}
