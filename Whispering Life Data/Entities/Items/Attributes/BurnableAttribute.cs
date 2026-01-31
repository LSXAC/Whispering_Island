using System;
using Godot;

[GlobalClass]
public partial class BurnableAttribute : ItemAttributeBase
{
    [Export]
    public int burntime = 60;

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("BURNTIME") + ": " + burntime + "s" + "\n";
    }
}
