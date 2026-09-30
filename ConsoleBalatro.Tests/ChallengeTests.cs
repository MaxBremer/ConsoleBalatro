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
using Xunit;

namespace ConsoleBalatro.Tests;

public class ChallengeTests : TestClassBase
{
    [Fact]
    public void Challenges_DefaultToFirstTwoUnlocked()
    {
        UnlockManager.ResetProgressToDefaults();

        Assert.True(ChallengeManager.IsUnlocked("THE OMELETTE"));
        Assert.True(ChallengeManager.IsUnlocked("15 MINUTE CITY"));
        Assert.False(ChallengeManager.IsUnlocked("RICH GET RICHER"));
    }

    [Fact]
    public void BeatingFirstTwoChallenges_UnlocksThirdChallengeThroughAchievement()
    {
        UnlockManager.ResetProgressToDefaults();

        Assert.True(UnlockManager.MarkChallengeBeaten("THE OMELETTE", saveImmediately: false));
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeWinId));
        Assert.False(ChallengeManager.IsUnlocked("RICH GET RICHER"));

        Assert.True(UnlockManager.MarkChallengeBeaten("15 MINUTE CITY", saveImmediately: false));

        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.FifteenMinuteCityChallengeWinId));
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.RichGetRicherChallengeUnlockId));
        Assert.True(ChallengeManager.IsUnlocked("RICH GET RICHER"));
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
        FlowHandler.ChallengeChosen("THE OMELETTE");
        FlowHandler.CurrentAnte = 8;
        FlowHandler.CurrentSelectedBlind = BlindType.BOSS;

        FlowHandler.IncrementBlind();

        Assert.True(ChallengeManager.IsBeaten("THE OMELETTE"));
        Assert.True(UnlockManager.IsAchievementAchieved(AchievementDb.OmeletteChallengeWinId));
    }

    [Fact]
    public void Omelette_StartsWithFiveEggs()
    {
        ResetEngineForTest();

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

    private ChallengeDefinition StartChallenge(string id)
    {
        ResetEngineForTest();

        var definition = ChallengeManager.ChallengeDb[id]();
        ChallengeManager.Begin(definition);

        return definition;
    }
}
