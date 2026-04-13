using System;
using Godot;

public partial class TranslationButton : Button
{
    [Export]
    public string label_translation_string;

    [Export]
    public bool has_different_label = false;

    [Export]
    public bool only_meta_text = false;

    [Export]
    public Label label;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;
        if (label_translation_string == null)
            return;
        UpdateText();
    }

    public override void _Pressed()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonSound();

        base._Pressed();
    }

    private void UpdateText()
    {
        if (only_meta_text)
        {
            TooltipText = TranslationServer.Translate(label_translation_string);
            return;
        }
        if (!has_different_label)
            Text = TranslationServer.Translate(label_translation_string);
        else if (label != null)
            label.Text = TranslationServer.Translate(label_translation_string);
    }
}
