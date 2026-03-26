using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Template für eine neue spezialisierte ProcessBuilding-Subklasse
/// Kopiere diese Datei und passe sie an deine Bedürfnisse an.
///
/// Schritte:
/// 1. Benenne die Klasse um (z.B. CombinerBuilding, BreweryBuilding)
/// 2. Definiere deine eigene SlotType enum
/// 3. Implementiere die abstrakten Methoden:
///    - GetRecipeFromInputSlot()
///    - SelectAndCheckCanCraft()
///    - GetUIUpdater()
///    - GetSlotIndexByPurpose()
/// 4. Optional: Überschreibe Hook-Methoden:
///    - OnProcessingComplete()
///    - OnProcessingTick()
///    - RefuelIfNeeded()
/// </summary>
public partial class TemplateProcessBuilding : ProcessBuilding
{
    /// <summary>
    /// Definiere deine Slot-Indizes hier
    /// </summary>
    public enum SlotType
    {
        INPUT = 0,
        OUTPUT = 1,
        FUEL = 2 // Optional, wenn nicht nötig kann auf -1 mapped werden
    }

    /// <summary>
    /// Liest das Rezept aus dem Input-Slot
    /// Wird aufgerufen während Verarbeitung läuft
    /// </summary>
    protected override IProcessingRecipe GetRecipeFromInputSlot()
    {
        int input_idx = (int)SlotType.INPUT;

        if (item_array[input_idx] == null)
            return null;

        ItemInfo input_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[input_idx].item_id
        ];

        // Beispiel: SmeltableAttribute, aber könnten auch andere Attribute sein
        SmeltableAttribute recipe = input_info?.GetAttributeOrNull<SmeltableAttribute>();
        return recipe;
    }

    /// <summary>
    /// Prüft ob eine Verarbeitung möglich ist
    /// Wird aufgerufen in _PhysicsProcess()
    /// </summary>
    protected override bool SelectAndCheckCanCraft()
    {
        int input_idx = (int)SlotType.INPUT;

        if (item_array[input_idx] == null)
            return false;

        IProcessingRecipe recipe = GetRecipeFromInputSlot();
        if (recipe == null)
            return false;

        // Weitere Validierung hier
        // z.B. Unlock-Requirements prüfen
        if (!recipe.IsUnlocked())
            return false;

        if (item_array[input_idx].amount < recipe.GetAmountToProcess())
            return false;

        // Prüfe Output-Slot Kompatibilität
        int output_idx = (int)SlotType.OUTPUT;
        ItemInfo output_info = recipe.GetOutputItem();

        if (item_array[output_idx] == null)
            return true;

        if (item_array[output_idx].item_id != (int)output_info.id)
            return false;

        return true;
    }

    /// <summary>
    /// Gibt den UI-Updater zurück
    /// </summary>
    protected override ProcessingTab GetUIUpdater()
    {
        // Hier an dein Tab anpassen (z.B. CombinerTab, BreweryTab)
        return FurnaceTab.instance; // Beispiel: nutze erst FurnaceTab für Testing
    }

    /// <summary>
    /// Mapped SlotPurpose zu Array-Index
    /// </summary>
    protected override int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        return purpose switch
        {
            SlotPurpose.INPUT => (int)SlotType.INPUT,
            SlotPurpose.OUTPUT => (int)SlotType.OUTPUT,
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            _ => -1
        };
    }

    /// <summary>
    /// Optional: Hook nach Verarbeitung abgeschlossen
    /// </summary>
    protected override void OnProcessingComplete(IProcessingRecipe recipe)
    {
        base.OnProcessingComplete(recipe);

        // Hier custom Logik nach Verarbeitung
        // z.B. Partikel-Effekte, Sounds, etc.
        GD.Print($"[{Name}] Verarbeitung abgeschlossen!");
    }

    /// <summary>
    /// Optional: Hook pro Timer-Tick während Verarbeitung
    /// </summary>
    protected override void OnProcessingTick(IProcessingRecipe recipe)
    {
        base.OnProcessingTick(recipe);

        // Hier custom Logik pro Tick
        // z.B. Energie-Verbrauch prüfen, Temperatur erhöhen, etc.
    }

    /// <summary>
    /// Optional: Custom Brennstoff-Logik
    /// Überschreibe um eigene Fuel-Rules zu implementieren
    /// </summary>
    protected override bool RefuelIfNeeded()
    {
        // Beispiel: Nutze base-Implementierung
        return base.RefuelIfNeeded();

        // Oder eigene Logik:
        // int fuel_idx = (int)SlotType.FUEL;
        // if (fuel_left >= 20) return true;
        // if (item_array[fuel_idx] == null) return false;
        // ... custom fuel logic ...
    }
}
