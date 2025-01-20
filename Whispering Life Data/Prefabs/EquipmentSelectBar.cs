using System;
using Godot;

public partial class EquipmentSelectBar : Container
{
    [Export]
    public Color normal_color,
        selected_color;

    [Export]
    public Slot[] select_slots = new Slot[4];

    public static int current_selected_slot = 0;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NUM1"))
            SelectSelectSlot(0);
        if (Input.IsActionJustPressed("NUM2"))
            SelectSelectSlot(1);
        if (Input.IsActionJustPressed("NUM3"))
            SelectSelectSlot(2);
        if (Input.IsActionJustPressed("NUM4"))
            SelectSelectSlot(3);
    }

    public void SelectSelectSlot(int index)
    {
        if (current_selected_slot == index)
            return;

        select_slots[current_selected_slot].GetParent().GetParent<ColorRect>().Color = normal_color;
        select_slots[index].GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot = index;
    }
}
