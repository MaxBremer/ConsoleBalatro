using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Tags;

namespace ConsoleBalatro.Engine.Pools.Rollables;

public sealed class TagRollableDefinition : RollableDefinition
{
    public TagRollableDefinition(TagType tagType)
    {
        TagType = tagType;
        Id = tagType.ToString();
    }

    public TagType TagType { get; }
    public override string Id { get; init; }
    public override ItemPool Pool => ItemPool.Tag;
    public override JokerRarity Rarity { get; init; } = JokerRarity.COMMON;
}
