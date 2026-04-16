using System;
using System.Diagnostics;
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
        Value = Mathf.DbToLinear(
            AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(bus.ToString()))
        );
    }

    public void OnValueChanged(float value)
    {
        AudioServer.SetBusVolumeDb(
            AudioServer.GetBusIndex(bus.ToString()),
            Mathf.LinearToDb(value)
        );
        Debug.Print("Sound:" + value + " | Bus: " + bus.ToString());
    }
}
