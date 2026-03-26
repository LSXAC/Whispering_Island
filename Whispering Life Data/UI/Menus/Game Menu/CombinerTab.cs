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
                    "Linker Rohstoff"
                ),
                new ProcessingSlotConfig(
                    "right_input",
                    SlotPurpose.INPUT,
                    right_slot,
                    "Rechter Rohstoff"
                ),
                new ProcessingSlotConfig("output", SlotPurpose.OUTPUT, output_slot, "Fuel Slot"),
                new ProcessingSlotConfig("fuel", SlotPurpose.FUEL, fuel_slot, "Kombiert Ergebnis")
            ]
        );
    }
}
