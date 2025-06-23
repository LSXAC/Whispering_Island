using System;
using Godot;

[GlobalClass]
public partial class BurnableAttribute : ItemAttribute
{
    [Export]
    public int burntime = 60;
}
