using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

/// <summary>
/// Spezialisierte Implementierung von ProcessBuilding für Schmelzöfen (Furnace)
/// Verarbeitet SmeltableAttribute Rezepte und BurnableAttribute Brennstoff
/// </summary>
public partial class FurnaceBuilding : ProcessBuilding
{
    public enum SlotType
    {
        IMPORT = 0,
        EXPORT = 1,
        FUEL = 2
    };

    public override void _Ready()
    {
        base._Ready();
        if (item_array.Length < 4)
        {
            System.Array.Resize(ref item_array, 4);
        }
    }

    protected override IProcessingRecipe GetRecipeFromInputSlot()
    {
        if (item_array[(int)SlotType.IMPORT] == null)
            return null;

        ItemInfo import_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.IMPORT].item_id
        ];

        SmeltableAttribute smeltable = import_info?.GetAttributeOrNull<SmeltableAttribute>();
        return smeltable;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        if (item_array[(int)SlotType.IMPORT] == null)
            return false;

        ItemInfo import_item_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.IMPORT].item_id
        ];
        SmeltableAttribute smeltable = import_item_info.GetAttributeOrNull<SmeltableAttribute>();

        if (smeltable == null)
            return false;

        if (smeltable.unlock_requirements != null && smeltable.unlock_requirements.Count > 0)
            if (!GlobalFunctions.CheckResearchRequirements(smeltable.unlock_requirements))
                return false;

        if (item_array[(int)SlotType.IMPORT].amount < smeltable.amount_to_smelt)
            return false;

        // Check if output slot is compatible
        if (item_array[(int)SlotType.EXPORT] == null)
            return true;
        else if (item_array[(int)SlotType.EXPORT].item_id != (int)smeltable.smelted_to_item.info.id)
            return false;

        return true;
    }

    protected override ProcessingTab GetUIUpdater()
    {
        return FurnaceTab.instance;
    }

    protected override int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        return purpose switch
        {
            SlotPurpose.INPUT => (int)SlotType.IMPORT,
            SlotPurpose.OUTPUT => (int)SlotType.EXPORT,
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            _ => -1
        };
    }

    protected override void OpenGameMenuTab()
    {
        Debug.Print("Open Furnace Tab");
        GameMenu.instance.OnOpenFurnaceTab();
    }
}
