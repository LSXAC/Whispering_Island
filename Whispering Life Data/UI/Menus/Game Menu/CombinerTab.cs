using System;
using Godot;

public partial class CombinerTab : ProcessingTab
{
    [Export]
    public Slot left_slot;

    [Export]
    public Slot right_slot;

    [Export]
    public Slot output_slot;

    [Export]
    public Slot fuel_slot;

    [Export]
    public Texture2D fuel_icon;

    public static CombinerTab instance = null;

    public override void _Ready()
    {
        instance = this;

        // Initialisiere mit 3 Slots statt 2
        InitializeSlots(
            [
                new ProcessingSlotConfig(
                    "left_input",
                    SlotPurpose.INPUT,
                    left_slot,
                    "Primärer Rohstoff"
                ),
                new ProcessingSlotConfig(
                    "right_input",
                    SlotPurpose.AUXILIARY,
                    right_slot,
                    "Sekundärer Rohstoff"
                ),
                new ProcessingSlotConfig(
                    "output",
                    SlotPurpose.OUTPUT,
                    output_slot,
                    "Kombiniertes Ergebnis"
                ),
                new ProcessingSlotConfig("fuel", SlotPurpose.FUEL, fuel_slot, "Brennstoff")
            ]
        );
    }

    protected override Texture2D GetFuelIcon()
    {
        return fuel_icon;
    }
}
