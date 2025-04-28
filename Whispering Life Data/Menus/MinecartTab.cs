using System;
using Godot;

public partial class MinecartTab : ColorRect
{
    [Export]
    public Label engine_status_label;

    [Export]
    public ChestInventory chest_inventory;
    public static Minecart current_minecart = null;

    public void OnVisiblityChange()
    {
        if (current_minecart == null)
            return;
        UpdateUI();
    }

    public void UpdateUI()
    {
        engine_status_label.Text = "Status: " + current_minecart.is_running;
    }

    public void StartEngine()
    {
        current_minecart.is_running = true;
    }

    public void StopEngine()
    {
        current_minecart.is_running = false;
    }
}
