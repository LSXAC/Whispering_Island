using System;
using Godot;

[GlobalClass]
public partial class ProcessedAttribute : ItemAttributeBase
{
    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("PROCESSED");
    }
}
