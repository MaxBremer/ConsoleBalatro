using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Challenges;
using ConsoleBalatro.Engine.Pools;
using ConsoleBalatro.Engine.Pools.Rollables;
using ConsoleBalatro.Engine.Pools.Rules;
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
}
