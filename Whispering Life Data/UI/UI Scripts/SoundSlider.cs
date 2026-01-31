using System;
using Godot;

public partial class SoundSlider : HSlider
{
    public enum BUS
    {
        Master,
        Music,
        SFX
    }

    [Export]
    public BUS bus;

    public void OnVisiblityChange()
    {
        Value = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(bus.ToString()));
    }

    public void OnValueChanged(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(bus.ToString()), value);
    }
}
