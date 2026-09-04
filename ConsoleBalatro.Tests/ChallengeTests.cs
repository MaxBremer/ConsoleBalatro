using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Challenges;
using ConsoleBalatro.Engine.Pools;
using ConsoleBalatro.Engine.Pools.Rollables;
using ConsoleBalatro.Engine.Pools.Rules;
using Xunit;

namespace ConsoleBalatro.Tests;

public class ChallengeTests : TestClassBase
{
    [Fact]
    public void Omelette_StartsWithFiveEggs()
    {
        ResetEngineForTest();

        FlowHandler.ChallengeChosen("THE_OMELETTE");

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
