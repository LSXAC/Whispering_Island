# ProcessBuilding Refactoring - Dokumentation

## Übersicht

ProcessBuilding wurde nach demselben Muster wie ProcessingTab extrahiert und generalisiert, um:
- ✅ **Vererbbar** zu sein (neue Machine-Typen einfach erstellen)
- ✅ **Rezepte dynamisch** zu laden (basierend auf Item-Attributen)
- ✅ **Verarbeitung generalisiert** mit Hooks für Speziallogik
- ✅ **100% Rückwärts-kompatibel** (alte ProcessBuilding-Referenzen funktionieren noch)

## Neue Architektur

### Klassen-Hierarchie

```
MachineBase (abstract base)
	↑
	│
ProcessBuildingBase (abstract, generisch)
	↑
	│
FurnaceBuilding (concrete, Furnace-spezifisch)
	↑
	│
ProcessBuilding (Compatibility-Wrapper, für alte Referenzen)
```

### Neue Dateien

1. **ProcessBuildingBase.cs** - Generische Basis-Klasse
   - Verwaltet Item-Array, Progress, Fuel
   - Generische Verarbeitungslogik in `OnCraftingTimerTimeout()`
   - Abstrakte Methoden für Subklassen:
	 - `GetRecipeFromInputSlot()` - Liest Rezept aus Input-Slot
	 - `SelectAndCheckCanCraft()` - Validierung
	 - `GetUIUpdater()` - Gibt UI-Updater zurück (FurnaceTab, etc.)
	 - `GetSlotIndexByPurpose()` - Maps SlotPurpose zu Array-Index
   - Hook-Methoden für Speziallogik:
	 - `OnProcessingComplete()` - Nach Verarbeitung
	 - `OnProcessingTick()` - Während Verarbeitung
	 - `RefuelIfNeeded()` - Brennstoff-Logik (überschreibbar)

2. **FurnaceBuilding.cs** - Spezialisiert für Furnace
   - Erbt von `ProcessBuildingBase`
   - Implementiert abstrakte Methoden für Furnace:
	 - `GetRecipeFromInputSlot()` liest SmeltableAttribute
	 - `SelectAndCheckCanCraft()` Furnace-Validierung
	 - `GetSlotIndexByPurpose()` maps IMPORT→0, EXPORT→1, FUEL→2
   - Behält alte `SlotType` enum für Kompatibilität

3. **ProcessBuilding.cs** - Compatibility-Wrapper
   - Erbt von FurnaceBuilding
   - Wrapper um FurnaceBuilding für alte Referenzen
   - Kann nach Migration entfernt werden

## Datenfluss

### Verarbeitung (OnCraftingTimerTimeout)

```
ProcessBuildingBase.OnCraftingTimerTimeout()
	↓
GetRecipeFromInputSlot() [abstrakt, von FurnaceBuilding implementiert]
	↓
IProcessingRecipe (SmeltableAttribute)
	↓
ExecuteRecipe() → Output hinzufügen, Input reduzieren
	↓
OnProcessingComplete() [Hook für Speziallogik]
```

### Refüeling (RefuelIfNeeded)

```
ProcessBuildingBase.RefuelIfNeeded()
	↓
item_array[FUEL_SLOT] lesen
	↓
BurnableAttribute.burntime extrahieren
	↓
fuel_left erhöhen, Item-Menge reduzieren
```

Diese Methode kann auch von Subklassen überschrieben werden für Custom-Fuel-Logik.

## Wie man neue Machine-Typen erstellt

### Beispiel: CombinerBuilding

```csharp
public partial class CombinerBuilding : ProcessBuildingBase
{
	public enum SlotType
	{
		INPUT_1 = 0,
		INPUT_2 = 1,
		OUTPUT = 2
	};

	protected override IProcessingRecipe GetRecipeFromInputSlot()
	{
		// Lese CombinerAttribute aus INPUT_1
		ItemInfo info1 = Inventory.ITEM_TYPES[
			(Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
		];
		CombinerAttribute recipe = info1?.GetAttributeOrNull<CombinerAttribute>();
		
		// Validiere dass INPUT_2 das erforderliche Item hat
		if (recipe != null && item_array[(int)SlotType.INPUT_2] != null)
		{
			ItemInfo info2 = Inventory.ITEM_TYPES[
				(Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
			];
			if (info2.id == recipe.required_item.info.id)
				return recipe;
		}
		return null;
	}

	protected override bool SelectAndCheckCanCraft()
	{
		// Custom Validierung für 2 Inputs
		// Prüfe beide Input-Slots
		return item_array[(int)SlotType.INPUT_1] != null 
			&& item_array[(int)SlotType.INPUT_2] != null;
	}

	protected override CombinerTab GetUIUpdater()
	{
		return CombinerTab.instance;
	}

	protected override int GetSlotIndexByPurpose(SlotPurpose purpose)
	{
		return purpose switch
		{
			SlotPurpose.INPUT => (int)SlotType.INPUT_1, // Primärer Input
			SlotPurpose.OUTPUT => (int)SlotType.OUTPUT,
			SlotPurpose.FUEL => -1, // Kein Fuel für Combiner
			_ => -1
		};
	}
}
```

### Beispiel: Custom Brewing Building mit eigenem Brennstoff-System

```csharp
public partial class BreweryBuilding : ProcessBuildingBase
{
	public enum SlotType { INPUT = 0, OUTPUT = 1, CATALYST = 2 };

	protected override bool RefuelIfNeeded()
	{
		// Catalyst wird als "Fuel" genutzt, aber mit anderen Rules
		int catalyst_idx = (int)SlotType.CATALYST;
		
		if (fuel_left >= CATALYST_THRESHOLD)
			return true;
		
		if (item_array[catalyst_idx] == null || item_array[catalyst_idx].amount <= 0)
			return false;

		// Catalyst gibt feste Brennwert, nicht von BurnableAttribute
		fuel_left += CATALYST_BURN_VALUE;
		item_array[catalyst_idx].amount--;
		return true;
	}
	
	// ... andere Methoden
}
```

## Migrations-Pfad

### Alt (noch funktionierend):
```csharp
ProcessBuilding pb = ...;
pb.item_array[(int)ProcessBuilding.SlotType.IMPORT] = ...;
```

### Neu (empfohlen):
```csharp
FurnaceBuilding furnace = ...;
furnace.item_array[(int)FurnaceBuilding.SlotType.IMPORT] = ...;

// Oder noch generischer (aber ProcessBuilding muss null-check sein):
var ui = furnace.GetUIUpdater(); // BaseProcessingTab
ui.SetReferenceBuilding(furnace);
```

## Slot-System mit SlotPurpose

```csharp
public enum SlotPurpose
{
	INPUT,      // Input-Material
	OUTPUT,     // Output-Material
	FUEL,       // Brennstoff
	AUXILIARY   // Für zukünftige Slots
}
```

`GetSlotIndexByPurpose()` wird von Subklassen implementiert um den tatsächlichen Array-Index zu liefern.

## Verarbeitungs-Loop

```
while (machine_enabled)
	↓
_PhysicsProcess() prüft SelectAndCheckCanCraft()
	↓
RefuelIfNeeded() wenn fuel_left < threshold
	↓
is_crafting = true
process_timer.Start()
	↓
OnCraftingTimerTimeout() wiederholt sich:
	- progress += 5
	- OnProcessingTick(recipe) [Hook]
	- if progress >= 100:
		ExecuteRecipe(recipe)
		OnProcessingComplete(recipe) [Hook]
```

## Save/Load System

Das Save/Load System bleibt in ProcessBuildingBase und nutzt:
- `MachineSave.furnace_slots[]` für Item-Array
- `MachineSave.fuel_left` für Brennstoff

Subklassen können `Load()` / `Save()` überschreiben um Custom-Attribute zu speichern.

## Testing

### Unit-Test Beispiele

```csharp
[Test]
public void TestFurnaceBuildingSmeltingLogic()
{
	var furnace = new FurnaceBuilding();
	// Furnace mit SmeltableAttribute setzen
	// Progress erhöhen bis 100
	// ExecuteRecipe() sollte aufgerufen werden
	Assert.AreEqual(expected_output, furnace.item_array[1]);
}

[Test]
public void TestCombinerBuildingWith2Inputs()
{
	var combiner = new CombinerBuilding();
	// Setze INPUT_1 und INPUT_2
	// Validierung sollte erfolgreich sein
	Assert.IsTrue(combiner.SelectAndCheckCanCraft());
}
```

### Manual Testing

1. **Furnace öffnen** → Items ein/ausladen - sollte identisch wie vorher sein
2. **Furnace verarbeiten** → Progress sollte sichtbar sein
3. **Save/Load** → Furnace-State sollte erhalten bleiben
4. **GameMenu.OnOpenProcessingTab()** → sollte FurnaceTab mit korrektem Building öffnen

## FAQ

**F: Kann ich ProcessBuilding noch verwenden?**  
A: Ja, ProcessBuilding ist ein Wrapper um FurnaceBuilding. Verwende lieber FurnaceBuilding direkt für neue Code.

**F: Wie kann ich ein Building ohne Fuel machen?**  
A: Überschreibe `RefuelIfNeeded()` um `true` zurückzugeben (immer genug Fuel), oder `GetSlotIndexByPurpose(SlotPurpose.FUEL)` um `-1` zurückzugeben.

**F: Kann ich während die Verarbeitung läuft zusätzliche Logik ausführen?**  
A: Ja, überschreibe `OnProcessingTick(recipe)` für Logik pro Timer-Tick, oder `OnProcessingComplete(recipe)` für Logik nach Abschluss.

**F: Wie verwalte ich mehrere Input-Slots?**  
A: Verwende `GetSlotIndexByPurpose()` um nur den Primären-Input zu prüfen, oder überschreibe `SelectAndCheckCanCraft()` um alle Inputs zu validieren.

---

## Checklist für CI/Builds

- [x] Keine Compile-Fehler
- [x] ProcessBuilding als Wrapper funktioniert
- [x] Alte Referenzen (ProcessBuilding.instance) funktionieren noch
- [ ] Tests für ProcessBuildingBase generische Logik
- [ ] Tests für FurnaceBuilding spezifische Konfiguration
- [ ] Integration-Test mit Furnace im Spiel
