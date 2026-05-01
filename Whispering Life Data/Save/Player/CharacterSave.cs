using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class CharacterSave : Resource
{
    [Export]
    public ItemSave[] inventory_items = new ItemSave[30];

    [Export]
    public ItemSave[] seed_inventory_items = new ItemSave[30];

    [Export]
    public ItemSave[] equipped_tools = new ItemSave[8];

    [Export]
    public ItemSave[] equipped_armor = new ItemSave[4];

    [Export]
    public Vector2 player_position = new Vector2(0, -160);

    [Export]
    public int health_value = 100;

    [Export]
    public float fatigue_value = 0;

    [Export]
    public ItemSave research_slot_item = new ItemSave();
}
