using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class FurnaceBuilding : ProcessBuilding
{
    [Export]
    public Array<SmeltableRecipe> smeltable_recipes = new Array<SmeltableRecipe>();

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
            System.Array.Resize(ref item_array, 4);
    }

    protected override ProcessingRecipe GetRecipeFromInputSlot()
    {
        if (selected_recipe != null)
        {
            GD.PrintErr($"[FURNACE] Using selected recipe");
            if (item_array[(int)SlotType.IMPORT] != null)
            {
                SmeltableRecipe recipe = selected_recipe as SmeltableRecipe;
                if (recipe != null)
                {
                    ItemInfo import_info = Inventory.ITEM_TYPES[
                        (Inventory.ITEM_ID)item_array[(int)SlotType.IMPORT].item_id
                    ];

                    if (
                        recipe.GetInputRequirement() != null
                        && recipe.GetInputRequirement().id == import_info.id
                        && item_array[(int)SlotType.IMPORT].amount >= recipe.GetAmountToProcess()
                    )
                    {
                        GD.PrintErr($"[FURNACE] ✅ Selected recipe matches!");
                        return recipe;
                    }
                    else
                    {
                        GD.PrintErr(
                            $"[FURNACE] Selected recipe mismatch: InputReq={recipe.GetInputRequirement()?.name}, ImportInfo={import_info.name}, Amount={item_array[(int)SlotType.IMPORT].amount}/{recipe.GetAmountToProcess()}"
                        );
                    }
                }
            }
            return null;
        }

        if (item_array[(int)SlotType.IMPORT] == null)
        {
            GD.PrintErr($"[FURNACE] Import slot empty");
            return null;
        }

        ItemInfo import_info_default = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.IMPORT].item_id
        ];
        GD.PrintErr(
            $"[FURNACE] Searching {smeltable_recipes.Count} recipes for {import_info_default.name}..."
        );

        foreach (SmeltableRecipe smeltable_recipe in smeltable_recipes)
        {
            if (
                smeltable_recipe != null
                && smeltable_recipe.GetInputRequirement() != null
                && smeltable_recipe.GetInputRequirement().id == import_info_default.id
                && smeltable_recipe.IsUnlocked()
                && item_array[(int)SlotType.IMPORT].amount >= smeltable_recipe.GetAmountToProcess()
            )
            {
                GD.PrintErr(
                    $"[FURNACE] ✅ Recipe found: {smeltable_recipe.GetInputRequirement().name} -> {smeltable_recipe.GetOutputItem().name}"
                );
                return smeltable_recipe;
            }
        }

        GD.PrintErr($"[FURNACE] ❌ No recipe found for {import_info_default.name}");
        return null;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        if (item_array[(int)SlotType.IMPORT] == null)
        {
            GD.PrintErr($"[SELECT] Import slot empty");
            return false;
        }

        ItemInfo import_item_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.IMPORT].item_id
        ];
        GD.PrintErr($"[SELECT] Checking for {import_item_info.name}...");

        SmeltableRecipe recipe = null;

        if (selected_recipe != null)
        {
            recipe = selected_recipe as SmeltableRecipe;
            if (recipe == null)
            {
                GD.PrintErr($"[SELECT] Selected recipe is not SmeltableRecipe!");
                return false;
            }

            if (
                recipe.GetInputRequirement() == null
                || recipe.GetInputRequirement().id != import_item_info.id
            )
            {
                GD.PrintErr($"[SELECT] Selected recipe input mismatch!");
                return false;
            }
            GD.PrintErr($"[SELECT] Using selected recipe");
        }
        else
        {
            foreach (SmeltableRecipe smelting_recipe in smeltable_recipes)
            {
                if (
                    smelting_recipe != null
                    && smelting_recipe.GetInputRequirement() != null
                    && smelting_recipe.GetInputRequirement().id == import_item_info.id
                    && smelting_recipe.IsUnlocked()
                    && item_array[(int)SlotType.IMPORT].amount
                        >= smelting_recipe.GetAmountToProcess()
                )
                {
                    recipe = smelting_recipe;
                    GD.PrintErr(
                        $"[SELECT] Found recipe: {smelting_recipe.GetInputRequirement().name}"
                    );
                    break;
                }
            }
        }

        if (recipe == null)
            return false;

        if (item_array[(int)SlotType.FUEL] == null && fuel_left <= 0)
            return false;

        if (item_array[(int)SlotType.IMPORT].amount < recipe.GetAmountToProcess())
            return false;

        if (item_array[(int)SlotType.EXPORT] == null)
            return true;

        ItemInfo expected_output = recipe.GetOutputItem();
        if (expected_output == null)
            return false;

        if (item_array[(int)SlotType.EXPORT].item_id != (int)expected_output.id)
            return false;
        else if (item_array[(int)SlotType.EXPORT].state != recipe.GetItemState())
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

    protected override bool CanItemFitInInputSlot(Inventory.ITEM_ID item_id)
    {
        if (selected_recipe == null)
            return true;

        SmeltableRecipe recipe = selected_recipe as SmeltableRecipe;
        if (recipe == null)
            return false;

        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (recipe.GetInputRequirement() == null)
            return false;

        return recipe.GetInputRequirement().id == item_info.id;
    }

    protected override void OpenGameMenuTab()
    {
        GameMenu.instance.OnOpenFurnaceTab();
    }
}
