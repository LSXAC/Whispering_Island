using System;
using Godot;

public partial class SettingsTab : ColorRect
{
    [Export]
    public OptionButton option_button;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

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
