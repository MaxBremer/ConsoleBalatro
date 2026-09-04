using ConsoleBalatro.Engine.Cards.Jokers;

namespace ConsoleBalatro.Engine.Challenges;

public static class ChallengeManager
{
    private static readonly Dictionary<string, ChallengeDefinition> Definitions =
        new(StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<ChallengeDefinition> All => Definitions.Values.ToList();
    public static ChallengeDefinition? CurrentChallenge { get; private set; }

    static ChallengeManager()
    {
        var omelette = new ChallengeDefinition
        {
            Id = "THE_OMELETTE",
            Name = "The Omelette",
            Description = "Start with five Eggs. Build their sell value, then decide when to crack them."
        };
        for (var i = 0; i < 5; i++)
            omelette.StartingJokers.Add(() => JokerDb.GenerateJokerCard("EGG"));
        Register(omelette);
    }

    public static void Register(ChallengeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.Id))
            throw new ArgumentException("A challenge must have an ID.", nameof(definition));
        Definitions.Add(definition.Id, definition);
    }

    public static bool TryGet(string id, out ChallengeDefinition definition) =>
        Definitions.TryGetValue(id, out definition!);

    public static void Begin(ChallengeDefinition definition)
    {
        CurrentChallenge = definition;
        definition.Apply();
    }

    public static void Clear() => CurrentChallenge = null;
}
