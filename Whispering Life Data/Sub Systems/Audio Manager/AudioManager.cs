using System;
using Godot;

public partial class AudioManager : Node2D
{
    [Export]
    public AudioStreamPlayer SFXPlayer;

    [Export]
    public AudioStreamPlayer MusicPlayer;

    [Export]
    public AudioStreamPlayer ButtonSoundPlayer;

    private static AudioManager instance;

    public override void _Ready()
    {
        if (instance == null)
            instance = this;
        else
            QueueFree();
    }

    public static AudioManager Instance => instance;

    public void PlaySFX(AudioStream audio)
    {
        if (SFXPlayer != null && audio != null)
        {
            SFXPlayer.Stream = audio;
            SFXPlayer.Play();
        }
    }

    public void SetSFX(AudioStream audio)
    {
        if (SFXPlayer != null && audio != null)
            SFXPlayer.Stream = audio;
    }

    public void StopSFX()
    {
        if (SFXPlayer != null)
            SFXPlayer.Stop();
    }

    public void PlayMusic(AudioStream audio, bool loop = true)
    {
        if (MusicPlayer != null && audio != null)
        {
            MusicPlayer.Stream = audio;
            MusicPlayer.Bus = "Music";
            if (MusicPlayer.Stream is AudioStreamOggVorbis oggStream)
                oggStream.Loop = loop;
            MusicPlayer.Play();
        }
    }

    public void SetMusic(AudioStream audio)
    {
        if (MusicPlayer != null && audio != null)
            MusicPlayer.Stream = audio;
    }

    public void StopMusic()
    {
        if (MusicPlayer != null)
            MusicPlayer.Stop();
    }

    public void PlayButtonSound()
    {
        if (ButtonSoundPlayer != null)
            ButtonSoundPlayer.Play();
    }

    public void SetButtonSound(AudioStream audio)
    {
        if (ButtonSoundPlayer != null && audio != null)
            ButtonSoundPlayer.Stream = audio;
    }
}
