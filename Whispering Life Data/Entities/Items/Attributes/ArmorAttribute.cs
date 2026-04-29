using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ArmorAttribute : WearableAttribute
{
    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("ARMOR");
    }
}
