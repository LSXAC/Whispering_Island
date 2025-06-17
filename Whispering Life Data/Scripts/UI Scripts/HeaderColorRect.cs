using System;
using Godot;

public partial class HeaderColorRect : ColorRect
{
    [Export]
    public string translation_string;

    public override void _Ready()
    {
        GetChild(0).GetNode<TranslationLabel>("Label").label_translation_string =
            translation_string;
        GetChild(0).GetNode<TranslationLabel>("Label").UpdateText();
    }
}
