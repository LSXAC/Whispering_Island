using System;
using Godot;

public partial class TranslationButton : Button
{
    [Export]
    public string label_translation_string;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;
        if (label_translation_string == null)
            return;
        UpdateText();
    }

    private void UpdateText()
    {
        Text = TranslationServer.Translate(label_translation_string);
    }
}
