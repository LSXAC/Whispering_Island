# ProcessingTab Refactoring - Dokumentation

## Übersicht

ProcessingTab wurde nach dem CraftingMenu-Muster extrahiert und generalisiert, um:
- ✅ **Vererbbar** zu sein (neue Tab-Typen einfach erstellen)
- ✅ **Dynamische Slots** zu unterstützen (unterschiedliche Anzahl von Eingängen/Ausgängen)
- ✅ **Rezepte dynamisch** zu laden (basierend auf Item-Attributen, nicht Code)

## Neue Architektur

### Klassen-Hierarchie

```
SlotUpdater (abstract base)
	↑
	│
BaseProcessingTab (abstract, generisch)
	↑
	│
FurnaceTab (concrete, Furnace-spezifisch)
	↑
	│
ProcessingTab (Compatibility-Wrapper, für alte Referenzen)
```

### Neue Dateien

1. **BaseProcessingTab.cs** - Generische Basis-Klasse
   - Verwaltet dynamische Slots via `ProcessingSlotConfig[]`
   - Generische Update/Clear-Logik mit Loop statt hardcoded Slots
   - Virtuelle Methoden für Erweiterung (z.B. `GetSlotIndexByPurpose()`)

2. **FurnaceTab.cs** - Spezialisiert für Furnace
   - Erbt von `BaseProcessingTab`
   - Definiert 3 Standard-Slots: IMPORT, EXPORT, FUEL
   - Behält alte `SlotType` enum für Kompatibilität

3. **ProcessingTab.cs** - Compatibility-Wrapper
   - Wrapper um FurnaceTab für alte Referenzen
   - Kann nach Migration entfernt werden

4. **ProcessingSlotConfig.cs** - Slot-Konfiguration
   - Struct mit Slot-Metadaten: Name, Zweck (INPUT/OUTPUT/FUEL), UI-Referenz
   - Enum `SlotPurpose` zur Kategorisierung

5. **IProcessingRecipe.cs** - Rezept-Interface
   - Standardisiert alle Rezept-Typen (SmeltableAttribute, zukünftige CombinerAttribute, etc.)
   - Methoden: `GetAmountToProcess()`, `GetOutputItem()`, `IsUnlocked()`, etc.

6. **SmeltableAttribute.cs** - Updated
   - Implementiert nun `IProcessingRecipe`
   - Kann als generisches Rezept behandelt werden

## Wie man neue ProcessingTab-Typen erstellt

### Beispiel: CombinerTab

```csharp
public partial class CombinerTab : BaseProcessingTab
{
	[Export]
	public Slot left_slot;
	
	[Export]
	public Slot right_slot;
	
	[Export]
	public Slot output_slot;

	public override void _Ready()
	{
		instance = this;
		
		// Initialisiere mit 3 Slots statt 2
		InitializeSlots(new ProcessingSlotConfig[]
		{
			new ProcessingSlotConfig("left_input", SlotPurpose.INPUT, left_slot, "Linker Rohstoff"),
			new ProcessingSlotConfig("right_input", SlotPurpose.INPUT, right_slot, "Rechter Rohstoff"),
			new ProcessingSlotConfig("output", SlotPurpose.OUTPUT, output_slot, "Kombiert Ergebnis")
		});
	}
}
```

### Beispiel: CombinerAttribute (Rezept)

```csharp
public partial class CombinerAttribute : ItemAttributeBase, IProcessingRecipe
{
	[Export]
	public Item required_item;
	
	[Export]
	public Item combined_result;

	public int GetAmountToProcess() => 1;
	public ItemInfo GetOutputItem() => combined_result?.info;
	public int GetProcessingTime() => 1500;
	public bool IsUnlocked() => true;
	// ... weitere IProcessingRecipe-Methoden
}
```

## Slot-Verwaltung

### Dynamisch Slots hinzufügen

```csharp
// In einer Subklasse von BaseProcessingTab:
protected void InitializeSlots(ProcessingSlotConfig[] configs)
{
	// Diese Klasse handhabt den Rest!
}

// Slots nach Zweck abrufen:
int import_index = GetSlotIndexByPurpose(SlotPurpose.INPUT);
int[] all_inputs = GetSlotIndicesByPurpose(SlotPurpose.INPUT); // mehrere Inputs möglich
```

### Slot-Logik generalisiert

- **ClearSlot(int index)** → Loop über alle Slots, nicht hardcoded
- **UpdateUI()** → Loop über alle Items in `item_array`
- **OvertakeItems()** → Loop über alle Slots beim Speichern

## Rezept-System

### Wie Rezepte geladen werden

1. ProcessBuilding liest Item aus IMPORT-Slot
2. Ruft `Item.GetAttributeOrNull<IProcessingRecipe>()` auf
3. Nutzt `IProcessingRecipe` für `GetAmountToProcess()`, `GetOutputItem()`, etc.
4. → Keine Abhängigkeit von spezifischen Attribut-Typen!

### Zukünftige Rezept-Typen

- `SmeltableAttribute` ✅ (schon implementiert)
- `CombinerAttribute` (einfach implementieren mit `IProcessingRecipe`)
- `AlchemyAttribute` (einfach implementieren mit `IProcessingRecipe`)
- `DistillerAttribute` (einfach implementieren mit `IProcessingRecipe`)

## Datenfluss

```
Item im Slot hat SmeltableAttribute
	↓
ProcessBuilding.SelectAndCheckCanCraft()
	↓ (liest SmeltableAttribute)
SmeltableAttribute implementiert IProcessingRecipe
	↓ (GetAmountToProcess, GetOutputItem, GetUnlockRequirements)
Furnace verarbeitet nach diesen Specs
	↓
UpdateUI() lädt Items basierend auf Slot-Index (nicht Slot-Namen!)
```

## Migrations-Pfad

### Alt (noch funktionierend):
```csharp
ProcessingTab.instance.SetReferenceBuilding(this);
item_array[(int)ProcessingTab.SlotType.IMPORT] = ...;
```

### Neu (empfohlen):
```csharp
FurnaceTab.instance.SetReferenceBuilding(this);
item_array[(int)FurnaceTab.SlotType.IMPORT] = ...;
// oder generisch:
int import_index = furnace_tab.GetSlotIndexByPurpose(SlotPurpose.INPUT);
item_array[import_index] = ...;
```

### ProcessingTab Wrapper wird entfernt (später)

## Erweiterungspunkte

### Zusätzliche virtuelle Methoden in BaseProcessingTab

- `OnRecipeChanged(IProcessingRecipe recipe)` - Wenn sich das Rezept ändert
- `OnSlotConfigured(int index)` - Wenn ein Slot initialisiert wird
- `ValidateSlotConfiguration()` - Vor der ersten Nutzung

## Testing

### Unit-Test Beispiel

```csharp
[Test]
public void TestFurnaceTabWithThreeSlots()
{
	var furnace = new FurnaceTab();
	furnace._Ready();
	
	// Prüfe, dass 3 Slots konfiguriert wurden
	Assert.AreEqual(3, furnace.slot_configs.Length);
	
	// Prüfe Slot-Zwecke
	var import_idx = furnace.GetSlotIndexByPurpose(SlotPurpose.INPUT);
	Assert.AreEqual(0, import_idx);
}
```

## FAQ

**F: Kann ich ProcessingTab noch verwenden?**
A: Ja, ProcessingTab ist ein Wrapper um FurnaceTab. Aber verwende lieber FurnaceTab direkt.

**F: Wie erstelle ich einen Tab mit nur 2 Slots?**
A: Erstelle eine Subklasse von BaseProcessingTab und rufe `InitializeSlots()` mit 2 Configs auf.

**F: Können mehrere Items gleichzeitig verarbeitet werden?**
A: Die aktuelle Item-Array-Struktur unterstützt nur 1 Item pro Slot. Mit `GetSlotIndicesByPurpose()` könntest du aber mehrere INPUT-Slots haben.

**F: Wie füge ich ein neues Rezept-Attribut hinzu?**
A: Erbe von `ItemAttributeBase` und implementiere `IProcessingRecipe`. Das war's!

---

## Checklist für CI/Builds

- [x] Keine Compile-Fehler
- [x] ProcessingTab als Wrapper funktioniert
- [x] Alte Referenzen (ProcessingTab.instance) funktionieren noch
- [ ] Tests für BaseProcessingTab generische Logik
- [ ] Tests für FurnaceTab spezifische Konfiguration
- [ ] Integration-Test mit ProcessBuilding
