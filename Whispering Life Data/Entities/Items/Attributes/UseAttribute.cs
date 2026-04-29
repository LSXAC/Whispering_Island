using System;
using Godot;

[GlobalClass]
public partial class UseAttribute : ItemAttributeBase
{
    // Potion/Effect properties
    [Export]
    public int stamina_restoration = 0;

    [Export]
    public int health_restoration = 0;

    [Export]
    public int mana_restoration = 0;

    [Export]
    public float effect_duration = 0f;

    public bool HasEffects()
    {
        return stamina_restoration > 0 || health_restoration > 0 || mana_restoration > 0;
    }

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("USE");
    }
}
