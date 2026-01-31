using System;
using Godot;

[GlobalClass]
public partial class ResourceAttribute : ItemAttributeBase
{
    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("RESOURCE") + "\n";
    }
}
