using System;
using System.Diagnostics;
using Godot;

public partial class ItemUseManager : Node2D
{
    public static ItemUseManager instance;

    public override void _Ready()
    {
        instance = this;
    }

    public void UseItem(Item item, Node target = null)
    {
        if (item?.info == null)
            return;

        // UseAttribute - Effekte anwenden
        UseAttribute use_attr = item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null && use_attr.HasEffects())
        {
            ApplyUseEffects(use_attr, target);
        }

        // Weitere Attribute die bei Verwendung reagieren
        // z.B. ConsumableAttribute, BuffAttribute, etc.
    }

    private void ApplyUseEffects(UseAttribute use_attr, Node target)
    {
        // Wenn kein Target, verwende Player
        if (target == null)
            target = PlayerUI.instance;

        Debug.Print("Item used!");
    }
}
