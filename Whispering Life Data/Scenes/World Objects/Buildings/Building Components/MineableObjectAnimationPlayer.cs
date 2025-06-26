using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class MineableObjectAnimationPlayer : SpriteAnimationPlayer
{
    [Export]
    public Array<Texture2D> textures;
}
