using Godot;

/// <summary>
/// Template für ein neues spezialisiertes ProcessingTab (z.B. für Combiner, Alchemie-Labor, etc.)
/// Kopiere diese Datei und passe sie an deine Bedürfnisse an.
///
/// Schritte:
/// 1. Benenne die Klasse um (z.B. CombinerTab, AlchemyLabTab)
/// 2. Füge deine [Export] Slots hinzu
/// 3. Passe die InitializeSlots() Aufrufe an
/// 4. Optional: Überschreibe virtuelle Methoden zum Anpassen des Verhaltens
/// </summary>
public partial class CustomProcessingTab : ProcessingTab
{
    // Beispiel: Falls dieser Tab mehrere Input-Slots brauchte:
    [Export]
    public Slot input_slot_1;

    [Export]
    public Slot input_slot_2;

    [Export]
    public Slot output_slot;

    // Optional: Ein Singleton-Instance für Zugriff von außen
    public static CustomProcessingTab instance = null;

    public enum SlotType
    {
        INPUT_1 = 0,
        INPUT_2 = 1,
        OUTPUT = 2
    }

    public override void _Ready()
    {
        instance = this;

        // Initialisiere die Slots mit der Custom-Konfiguration
        InitializeSlots(
            new ProcessingSlotConfig[]
            {
                new ProcessingSlotConfig(
                    "input_1",
                    SlotPurpose.INPUT,
                    input_slot_1,
                    "Erstes Input-Material"
                ),
                new ProcessingSlotConfig(
                    "input_2",
                    SlotPurpose.INPUT,
                    input_slot_2,
                    "Zweites Input-Material"
                ),
                new ProcessingSlotConfig(
                    "output",
                    SlotPurpose.OUTPUT,
                    output_slot,
                    "Kombiniertes Ergebnis"
                )
            }
        );
    }

    /// <summary>
    /// Optional: Überschreibe Methoden um Custom-Verhalten hinzuzufügen
    /// Beispiel: Custom Update-Logik für 2 Inputs
    /// </summary>
    public override void UpdateUI()
    {
        // Rufe die Basis-Implementierung auf
        base.UpdateUI();

        // Hier kannst du zusätzliche Custom-Logik hinzufügen
        // z.B. spezielle Beschreibung basierend auf 2 Input-Items
        DescribeInputCombination();
    }

    private void DescribeInputCombination()
    {
        var input1_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        var input2_idx =
            GetSlotIndicesByPurpose(SlotPurpose.INPUT).Length > 1
                ? GetSlotIndicesByPurpose(SlotPurpose.INPUT)[1]
                : -1;

        // Hier könnte man beispielweise prüfen:
        // - Sind beide Input-Slotsmit Items gefüllt?
        // - Passt das Rezept (IProcessingRecipe) zu beiden?
    }

    /// <summary>
    /// Hilfsmethode um beide Input-Items zu prüfen
    /// </summary>
    public bool BothInputSlotsFilled()
    {
        var input_indices = GetSlotIndicesByPurpose(SlotPurpose.INPUT);
        foreach (var idx in input_indices)
        {
            if (process_building?.item_array[idx] == null)
                return false;
        }
        return true;
    }
}
