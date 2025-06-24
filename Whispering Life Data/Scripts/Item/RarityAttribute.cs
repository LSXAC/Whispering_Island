using System;
using Godot;

public partial class RarityAttribute : ItemAttributeBase
{
    [Export]
    public Rarity rarity = Rarity.Common;

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
