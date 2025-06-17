using System;
using System.Diagnostics;
using Godot;

public partial class SettingsTab : ColorRect
{
    [Export]
    public OptionButton option_button;

    // Called when the node enters the scene tree for the first time.

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        GetOption();
    }

    private void GetOption()
    {
        Debug.Print(TranslationServer.GetLocale());
        switch (TranslationServer.GetLocale())
        {
            case "en":
                option_button.Selected = 0;
                break;
            case "de":
                option_button.Selected = 1;
                break;
        }
    }

    public void OnItemSelected(int index)
    {
        string language = "";
        switch (index)
        {
            case 0:
                language = "en";
                break;
            case 1:
                language = "de";
                break;
        }
        TranslationServer.SetLocale(language);
    }
}
