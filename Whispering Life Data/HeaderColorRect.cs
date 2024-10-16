using System;
using Godot;

public partial class HeaderColorRect : ColorRect
{
    [Export]
    public string translation_string;

    public override void _Ready()
    {
        GetNode<TranslationLabel>("Label").label_translation_string = translation_string;
        GetNode<TranslationLabel>("Label").UpdateText();
    }
}
