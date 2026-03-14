using System;
using Godot;

public partial class MagicPowerPanel : Panel
{
    [Export]
    public Label current_magic_power_label,
        latest_magic_power_label;

    public static MagicPowerPanel instance;

    public override void _Ready()
    {
        instance = this;
        current_magic_power_label.Text = "Magic Power: N/A";
        latest_magic_power_label.Text = "/";
    }

    public void UpdateMagicPowerUI(float current, float max)
    {
        current_magic_power_label.Text = $"Magic Power: {current:0.##} of {max}";
    }

    public void UpdateLatestMagicPowerUI(float amount)
    {
        latest_magic_power_label.Text = "" + amount;
    }
}
