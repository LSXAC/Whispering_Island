using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ItemRowManager : HBoxContainer
{
    private PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
    );

    public void SetResourcesOnUI(Array<Item> items, bool no_dev_list = false)
    {
        Debug.Print($"[ItemRowManager.SetResourcesOnUI] START with {items.Count} items");
        ClearChildren();
        Array<Item> items_to_use = null;
        if (!no_dev_list)
            items_to_use = GlobalFunctions.GetNormalListOrDevList(items);
        else
            items_to_use = items;

        if (Logger.NodeIsNull(items_to_use) || Logger.ListHasZeroItems(items_to_use))
            return;

        foreach (Item item in items_to_use)
        {
            h_box_item hbc_c = CreateHBoxItem(item);
            Array<Item> i_list = PlayerInventoryUI.instance?.GetItemFromListOrNull(
                PlayerInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            Array<Item> seed_list = SeedInventoryUI.instance?.GetItemFromListOrNull(
                SeedInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            Item item_ref = item.Clone();
            item_ref.amount = (int)(item_ref.amount * GameManager.difficulty_multiplier);

            int amount_of_item = 0;
            if (i_list != null)
            {
                foreach (Item i in i_list)
                    amount_of_item += i.amount;
            }

            if (seed_list != null)
            {
                foreach (Item i in seed_list)
                    amount_of_item += i.amount;
            }

            if (amount_of_item >= item_ref.amount && item_ref.amount > 0)
                hbc_c.ChangeColor(global::h_box_item.colorType.white);

            AddChild(hbc_c);
        }
    }

    public bool CheckEnoughResources(Array<Item> items)
    {
        if (Logger.NodeIsNull(items) || Logger.ListHasZeroItems(items))
        {
            SetTimesToBuildLabel(0);
            return false;
        }
        Array<Item> items_to_use = GlobalFunctions.GetNormalListOrDevList(items);

        int different_item_types = 0;
        Dictionary<Item, int> amount_of_each_item = new Dictionary<Item, int>();

        foreach (Item item in items_to_use)
        {
            Array<Item> i_list = PlayerInventoryUI.instance?.GetItemFromListOrNull(
                PlayerInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            Array<Item> seed_list = SeedInventoryUI.instance?.GetItemFromListOrNull(
                SeedInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            int amount_of_item = 0;
            if (i_list != null)
            {
                foreach (Item i in i_list)
                    amount_of_item += i.amount;
            }

            if (seed_list != null)
            {
                foreach (Item i in seed_list)
                    amount_of_item += i.amount;
            }

            if (amount_of_item >= item.amount && item.amount > 0)
            {
                amount_of_each_item[item] = amount_of_item / item.amount;
                different_item_types++;
            }
        }

        if (different_item_types == items_to_use.Count)
        {
            int times = amount_of_each_item.Values.Min();
            SetTimesToBuildLabel(times);
            return true;
        }
        SetTimesToBuildLabel(0);
        return false;
    }

    private void SetTimesToBuildLabel(int times)
    {
        if (IsInstanceValid(PlayerUI.instance?.times_to_build_left_label))
            PlayerUI.instance?.times_to_build_left_label?.Set(
                "text",
                $"> {times}x {TranslationServer.Translate("PLAYERUI_TIMES_LEFT_TO_BUILD")}"
            );
    }

    private void ClearChildren()
    {
        foreach (Control c in GetChildren())
            c.Free();
    }

    private h_box_item CreateHBoxItem(Item item)
    {
        h_box_item hbi = (h_box_item)h_box_item.Instantiate();
        hbi.InitItemUI(item);
        hbi.ChangeColor(global::h_box_item.colorType.red);
        return hbi;
    }
}
