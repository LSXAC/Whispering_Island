using System;
using Godot;

public partial class MagicPowerPanel : Panel
{
    [Export]
    public Label current_magic_power_label,
        latest_magic_power_label;

    public override void _Ready()
    {
        UpdateMagicPowerUI();
        latest_magic_power_label.Text = "/";
    }

    public void UpdateMagicPowerUI()
    {
        if (MagicPowerManager.instance == null)
        {
            current_magic_power_label.Text = "Magic Power: N/A";
            return;
        }
        current_magic_power_label.Text =
            $"Magic Power: {MagicPowerManager.instance.GetCurrentMagicPower():0.##}/{MagicPowerManager.instance.max_magical_power}";
    }

    public void UpdateLatestMagicPowerUI(float amount)
    {
        latest_magic_power_label.Text = "" + amount;
    }
}
