using System;
using Godot;

/// <summary>
/// Spezialisierte Implementierung von ProcessingTab für Furnace/Schmelzofen
/// Konfiguriert 3 Slots: INPUT (Rohstoff), OUTPUT (Ergebnis), FUEL (Brennstoff)
/// </summary>
public partial class FurnaceTab : ProcessingTab
{
    [Export]
    public Slot export_slot;

    [Export]
    public Slot import_slot;

    [Export]
    public Slot fuel_slot;

    [Export]
    public Texture2D fuel_icon;

    public static FurnaceTab instance = null;

    public enum SlotType
    {
        IMPORT = 0,
        EXPORT = 1,
        FUEL = 2
    }

    public override void _Ready()
    {
        instance = this;

        // Initialisiere die Slots mit der Standard-Furnace-Konfiguration
        InitializeSlots(
            [
                new ProcessingSlotConfig(
                    "import",
                    SlotPurpose.INPUT,
                    import_slot,
                    "Rohstoff zum Schmelzen"
                ),
                new ProcessingSlotConfig(
                    "export",
                    SlotPurpose.OUTPUT,
                    export_slot,
                    "Geschmolzenes Material"
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
