using System;
using Godot;

[GlobalClass]
public partial class WearableAttribute : ItemAttribute
{
    [Export]
    public TYPE type = TYPE.NONE; // head, chest, legs, feet, hands, back, face, neck, waist, wrist

    [Export]
    public int armor_points = 0; // Armor value of the wearable item

    public enum TYPE
    {
        NONE,
        HEAD,
        CHEST,
        LEGS,
        FEET
    }
}
