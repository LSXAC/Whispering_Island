using System;
using Godot;

public partial class TranslationPanel : Panel
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
        if (label_translation_string == "")
            return;
        Name = TranslationServer.Translate(label_translation_string);
    }
}
