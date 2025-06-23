using System;
using System.Security.Cryptography.X509Certificates;
using Godot;

[GlobalClass]
public partial class ItemAttribute : Resource
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
