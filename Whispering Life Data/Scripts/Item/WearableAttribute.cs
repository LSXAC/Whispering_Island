using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WearableAttribute : ItemAttributeBase
{
    [Export]
    public SLOT_TYPE slot_type = SLOT_TYPE.NONE; // head, chest, legs, feet, hands, back, face, neck, waist, wrist

    [Export]
    public int durability = 100;

    [Export]
    public Array<PlayerStat> stats = new Array<PlayerStat>();

    public enum SLOT_TYPE
    {
        NONE,
        HEAD,
        CHEST,
        LEGS,
        FEET,
        HAND
    }

    public override string GetNameOfAttribute()
    {
        return "EMPTY WEARABLE \n";
    }
}
