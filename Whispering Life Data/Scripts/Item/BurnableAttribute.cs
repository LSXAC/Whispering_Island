using System;
using Godot;

[GlobalClass]
public partial class BurnableAttribute : ItemAttributeBase
{
    [Export]
    public int burntime = 60;
}
