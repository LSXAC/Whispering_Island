using System;
using Godot;

[GlobalClass]
public partial class UseAttribute : ItemAttributeBase
{
    [Export]
    public Callable on_use_callable;

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("USE");
    }
}
