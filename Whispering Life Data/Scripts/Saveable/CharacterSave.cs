using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class CharacterSave : Resource
{
    [Export]
    public ItemSave[] inventory_items = new ItemSave[20];

    [Export]
    public Vector2 player_position = new Vector2();
}
