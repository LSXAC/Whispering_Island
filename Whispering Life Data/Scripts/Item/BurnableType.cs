using System;
using Godot;

[GlobalClass]
public partial class BurnableType : ItemType
{
    [Export]
    public int burntime = 60;
}
