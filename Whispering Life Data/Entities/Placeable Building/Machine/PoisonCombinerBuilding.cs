using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class PoisonCombinerBuilding : ProcessBuilding
{
    public enum SlotType
    {
        INPUT_1 = 0,
        OUTPUT = 2,
        FUEL = 3,
        INPUT_2 = 1,
    };

    public override void _Ready()
    {
        base._Ready();
        // Resizes item_array to 4 slots for Combiner (INPUT_1, INPUT_2, OUTPUT, FUEL)
        if (item_array.Length < 4)
        {
            System.Array.Resize(ref item_array, 4);
        }
    }

    protected override IProcessingRecipe GetRecipeFromInputSlot()
    {
        // Lese CombinerAttribute aus INPUT_1
        ItemInfo info1 = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
        ];
        CombinerAttribute recipe = info1?.GetAttributeOrNull<CombinerAttribute>();

        // Validiere dass INPUT_2 ein kompatibles Item ist
        if (recipe != null && item_array[(int)SlotType.INPUT_2] != null)
        {
            ItemInfo info2 = Inventory.ITEM_TYPES[
                (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
            ];

            // Prüfe ob INPUT_2 in der Liste der kompatiblen Items ist
            if (recipe.compatible_items != null)
            {
                foreach (Item compatible_item in recipe.compatible_items)
                {
                    if (compatible_item != null && compatible_item.info.id == info2.id)
                        return recipe;
                }
            }
        }
        return null;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        // Prüfe ob INPUT_1 vorhanden ist
        if (item_array[(int)SlotType.INPUT_1] == null)
            return false;

        ItemInfo input1_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
        ];
        CombinerAttribute recipe = input1_info?.GetAttributeOrNull<CombinerAttribute>();

        // Prüfe ob Rezept existiert
        if (recipe == null)
            return false;

        // Prüfe Unlock-Anforderungen
        if (recipe.unlock_requirements != null && recipe.unlock_requirements.Count > 0)
            if (!GlobalFunctions.CheckResearchRequirements(recipe.unlock_requirements))
                return false;

        // Prüfe ob genug Items zum Verarbeiten vorhanden sind
        if (item_array[(int)SlotType.INPUT_1].amount < recipe.amount_to_produce)
            return false;

        // Prüfe ob INPUT_2 vorhanden und kompatibel ist
        if (item_array[(int)SlotType.INPUT_2] == null)
            return false;

        ItemInfo input2_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
        ];

        bool is_compatible = false;
        if (recipe.compatible_items != null)
        {
            foreach (Item compatible_item in recipe.compatible_items)
            {
                if (compatible_item != null && compatible_item.info.id == input2_info.id)
                {
                    is_compatible = true;
                    break;
                }
            }
        }

        if (!is_compatible)
            return false;

        // Check if output slot is compatible
        if (item_array[(int)SlotType.OUTPUT] == null)
            return true;
        else if (item_array[(int)SlotType.OUTPUT].item_id != (int)recipe.output_item.info.id)
            return false;

        return true;
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
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            SlotPurpose.AUXILIARY => (int)SlotType.INPUT_2, // Sekundärer Input
            _ => -1
        };
    }

    protected override void OpenGameMenuTab()
    {
        Debug.Print("Open Poison Combiner Tab");
        GameMenu.instance.OnOpenPoisonCombinerTab();
    }
}
