using System;
using System.Diagnostics;
using Godot;

public partial class SettingsTab : ColorRect
{
    [Export]
    public OptionButton option_button,
        window_mode_button;

    [Export]
    public AudioStreamPlayer master_sound_player;

    [Export]
    public AudioStreamPlayer sfx_sound_player;

    [Export]
    public AudioStreamPlayer music_sound_player;

    [Export]
    public HBoxContainer exit_container;

    [Export]
    public Button exit_button;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        GetOption();
    }

    public void OnVisiblityChanged()
    {
        window_mode_button.Selected = (int)DisplayServer.WindowGetMode();
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

    //TODO: Translation for OptionButton Items
    public void ToggleMaximizedWindow(int index)
    {
        DisplayServer.WindowSetMode((DisplayServer.WindowMode)index);
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

    public void PlaySoundMaster()
    {
        if (master_sound_player == null)
            return;

        AudioManager.Instance.PlaySound(master_sound_player, "Master");
    }

    public void PlaySoundSFX()
    {
        if (sfx_sound_player == null)
            return;

        AudioManager.Instance.PlaySound(sfx_sound_player, "SFX");
    }

    public void PlaySoundMusic()
    {
        if (music_sound_player == null)
            return;

        AudioManager.Instance.PlaySound(music_sound_player, "Music");
    }
}
