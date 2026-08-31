using ConsoleBalatro.Engine.Challenges;

namespace ConsoleBalatro.Engine.Pools.Rules;

/// <summary>Applies the active challenge's restrictions to every content roll.</summary>
public sealed class ChallengePoolRule : IMarketPoolRule
{
    public int Priority => -1000;

    public void ModifyCandidates(MarketPoolContext context)
    {
        var challenge = ChallengeManager.CurrentChallenge;
        if (challenge == null)
            return;

        if (challenge.BannedGenerationSources.Contains(context.Source))
        {
            context.Candidates.Clear();
            return;
        }

        if (challenge.BannedPoolItems.TryGetValue(context.Pool, out var banned))
            context.Candidates.RemoveAll(candidate => banned.Contains(candidate.Definition.Id));
    }
}
