public class ElementalReactionData : EventData
{
    public int Point { get; set; }
    public bool IsCritical { get; set; }
    public int TurnsRemaining { get; set; }
    public ReactionType CurrentReactionType { get; set; }

    public ElementalReactionData(int point, bool isCritical, ElementType pyro, int turnsRemaining, Character target, ReactionType currentReactionType)
    {
        Point = point;
        IsCritical = isCritical;
        TurnsRemaining = turnsRemaining;
        CurrentReactionType = currentReactionType;
    }

}
