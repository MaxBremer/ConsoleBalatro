using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Pools;

namespace ConsoleBalatro.Engine.Challenges;

/// <summary>
/// Declarative description of a challenge run. New challenges can combine starting
/// cards, deck changes, pool restrictions, boss restrictions, and arbitrary setup.
/// </summary>
public sealed class ChallengeDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string BaseDeck { get; init; } = "RED";
    public List<Func<Card>> StartingJokers { get; } = [];
    public List<Func<Card>> StartingConsumables { get; } = [];
    public Action<CardZone>? ModifyStartingDeck { get; init; }
    public Action? ApplyRules { get; init; }
    public Dictionary<ItemPool, HashSet<string>> BannedPoolItems { get; } = [];
    public HashSet<GenerationSource> BannedGenerationSources { get; } = [];
    public HashSet<string> AllowedBossBlinds { get; } = new(StringComparer.OrdinalIgnoreCase);

    internal void Apply()
    {
        if (ZoneManager.DeckZone != null)
            ModifyStartingDeck?.Invoke(ZoneManager.DeckZone);

        foreach (var create in StartingJokers)
            ZoneManager.JokerZone?.AddCard(create());
        foreach (var create in StartingConsumables)
            ZoneManager.ConsumableZone?.AddCard(create());

        ApplyRules?.Invoke();
    }
}
