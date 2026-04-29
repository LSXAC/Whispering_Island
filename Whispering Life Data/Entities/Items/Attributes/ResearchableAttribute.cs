using System;
using Godot;

[GlobalClass]
public partial class ResearchableAttribute : ItemAttributeBase
{
    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("RESEARCHABLE");
    }
}
