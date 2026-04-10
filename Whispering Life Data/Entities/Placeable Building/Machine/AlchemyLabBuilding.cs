using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class AlchemyLabBuilding : ProcessBuilding
{
    public enum SlotType
    {
        PRIMARY_INPUT = 0,
        SECONDARY_INPUT = 1,
        OUTPUT = 2,
        FUEL = 3,
    };

    [Export]
    public Array<StateChangingRecipe> state_changing_recipes = new Array<StateChangingRecipe>();

    public override void _Ready()
    {
        base._Ready();

        if (item_array.Length < 4)
            System.Array.Resize(ref item_array, 4);

        // Kopiere state_changing_recipes in die generische recipes Array
        CollectAvailableRecipes();
    }

    protected override void CollectAvailableRecipes()
    {
        recipes.Clear();
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            recipes.Add(recipe);
        }
    }

    protected override ProcessingRecipe GetRecipeFromInputSlot()
    {
        if (
            item_array[(int)SlotType.PRIMARY_INPUT] == null
            || item_array[(int)SlotType.SECONDARY_INPUT] == null
        )
            return null;

        ItemInfo info1 = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];
        ItemInfo info2 = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        // Suche nach passendem StateChangingRecipe
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.IsUnlocked())
            {
                if (
                    recipe.CanCombine(info1, info2)
                    && item_array[(int)SlotType.PRIMARY_INPUT].amount >= recipe.GetAmountToProcess()
                )
                    return recipe;
            }
        }

        return null;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        if (
            item_array[(int)SlotType.PRIMARY_INPUT] == null
            || item_array[(int)SlotType.SECONDARY_INPUT] == null
        )
            return false;

        ItemInfo input1_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];
        ItemInfo input2_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        ProcessingRecipe recipe = null;

        foreach (StateChangingRecipe state_recipe in state_changing_recipes)
        {
            if (
                state_recipe != null
                && state_recipe.IsUnlocked()
                && state_recipe.CanCombine(input1_info, input2_info)
                && item_array[(int)SlotType.PRIMARY_INPUT].amount
                    >= state_recipe.GetAmountToProcess()
            )
            {
                recipe = state_recipe;
                break;
            }
        }

        if (recipe == null)
            return false;

        if (item_array[(int)SlotType.FUEL] == null && fuel_left <= 0)
            return false;

        if (item_array[(int)SlotType.PRIMARY_INPUT].amount < recipe.GetAmountToProcess())
            return false;

        // Für StateChangingRecipe: Output Item ist vom Primary Input abhängig
        if (recipe is StateChangingRecipe)
        {
            // Prüfe ob Output Slot leer ist oder das gleiche Item mit gleichem State enthält
            if (item_array[(int)SlotType.OUTPUT] != null)
            {
                int primary_item_id = item_array[(int)SlotType.PRIMARY_INPUT].item_id;
                int output_state = recipe.GetItemState();

                if (
                    item_array[(int)SlotType.OUTPUT].item_id != primary_item_id
                    || item_array[(int)SlotType.OUTPUT].state != output_state
                )
                {
                    // Output Slot hat anderes Item - Craft nicht möglich
                    return false;
                }
            }
            // StateChangingRecipe kann crafted werden, wenn Output Slot passt
            return true;
        }

        ItemInfo expected_output = recipe.GetOutputItem();

        if (expected_output != null)
        {
            if (item_array[(int)SlotType.OUTPUT] == null)
                return true;
            else if (item_array[(int)SlotType.OUTPUT].item_id != (int)expected_output.id)
                return false;
            else if (item_array[(int)SlotType.OUTPUT].state != recipe.GetItemState())
                return false;
        }

        return true;
    }

    protected override ProcessingTab GetUIUpdater()
    {
        return AlchemyLabTab.instance;
    }

    public override int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        return purpose switch
        {
            SlotPurpose.INPUT => (int)SlotType.PRIMARY_INPUT,
            SlotPurpose.OUTPUT => (int)SlotType.OUTPUT,
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            SlotPurpose.AUXILIARY => (int)SlotType.SECONDARY_INPUT,
            _ => -1
        };
    }

    protected override bool CanItemFitInInputSlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (item_array[(int)SlotType.SECONDARY_INPUT] == null)
            return true;

        ItemInfo secondary_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        // Prüfe ob Item mit sekundärem Input kompatibel ist
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.CanCombine(item_info, secondary_info))
                return true;
        }

        return false;
    }

    protected override bool CanItemFitInAuxiliarySlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (item_array[(int)SlotType.PRIMARY_INPUT] == null)
            return true;

        ItemInfo primary_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];

        // Prüfe ob Item mit primärem Input kompatibel ist
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.CanCombine(primary_info, item_info))
                return true;
        }

        return false;
    }

    protected override void OpenGameMenuTab()
    {
        GameMenu.instance.OnOpenAlchemyLabTab();
    }

    protected override void ExecuteRecipe(ProcessingRecipe recipe)
    {
        // Für StateChangingRecipe: Primary Input Item mit neuem State als Output
        if (recipe is StateChangingRecipe state_recipe)
        {
            int output_idx = (int)SlotType.OUTPUT;
            int primary_idx = (int)SlotType.PRIMARY_INPUT;
            int secondary_idx = (int)SlotType.SECONDARY_INPUT;

            // Output Item vom Primary Input (Slot 1) nehmen - das ist das variable Item
            if (item_array[primary_idx] != null)
            {
                int output_item_id = item_array[primary_idx].item_id;
                int output_amount = recipe.GetAmountToProduce();
                int output_state = recipe.GetItemState();

                if (item_array[output_idx] == null)
                {
                    // Output Slot ist leer - neues Item erstellen
                    item_array[output_idx] = new ItemSave(
                        output_item_id,
                        output_amount,
                        -1,
                        output_state
                    );
                }
                else if (
                    item_array[output_idx].item_id == output_item_id
                    && item_array[output_idx].state == output_state
                )
                {
                    // Output existiert bereits mit gleichem Item und State - addieren
                    item_array[output_idx].amount += output_amount;
                }
                else
                {
                    // Output Slot hat ein anderes Item - nicht überschreiben
                    return;
                }
            }

            // Slot 1 um 1 reduzieren
            if (item_array[primary_idx] != null)
            {
                item_array[primary_idx].amount -= 1;
            }

            // Slot 2 um 1 reduzieren
            if (item_array[secondary_idx] != null)
            {
                item_array[secondary_idx].amount -= 1;
            }

            is_crafting = false;
            process_timer.Stop();
            progress = 0;

            var ui_updater = GetUIUpdater();
            if (ui_updater != null && ui_updater.process_building == this)
            {
                ui_updater.SetMachineProgressbar(progress);
                ui_updater.UpdateUI();
            }

            OnProcessingComplete(recipe);
        }
        else
        {
            // Für normale Rezepte: base-Methode verwenden
            base.ExecuteRecipe(recipe);
        }
    }
}
