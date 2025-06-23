using System;
using Godot;

public partial class WearableAttribute : ItemAttribute
{
    [Export]
    public TYPE type = TYPE.NONE; // head, chest, legs, feet, hands, back, face, neck, waist, wrist

    [Export]
    public int armor = 0; // Armor value of the wearable item

    [Export]
    public int durability = 100; // Durability of the wearable item

    [Export]
    public string material = "fabric"; // Material type of the wearable item

    public enum TYPE
    {
        NONE,
        HEAD,
        CHEST,
        LEGS,
        FEET
    }
}
