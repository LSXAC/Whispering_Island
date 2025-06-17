using System;
using Godot;

public partial class TranslationRichTextLabel : RichTextLabel
{
    [Export]
    public string label_translation_string;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateText();
    }

    public void UpdateText()
    {
        Text = TranslationServer.Translate(label_translation_string);
    }
}
