using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ItemRowManager : HBoxContainer
{
    private PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/h_box_item_menu_line.tscn"
    );

    public void SetResourcesOnUI(Array<Item> items)
    {
        ClearChildren();
        Array<Item> items_to_use = GetNormalListOrDevList(items);
        if (Logger.NodeIsNull(items_to_use) || Logger.ListHasZeroItems(items_to_use))
            return;

        foreach (Item item in items_to_use)
        {
            h_box_item hbc_c = CreateHBoxItem(item);
            Array<Item> i_list = PlayerInventoryUI.instance?.GetItemFromListOrNull(
                PlayerInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            int amount_of_item = 0;
            if (i_list != null)
            {
                foreach (Item i in i_list)
                    amount_of_item += i.amount;

                if (amount_of_item >= item.amount && item.amount > 0)
                    hbc_c.ChangeColor(global::h_box_item.colorType.white);
            }
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
        Array<Item> items_to_use = GetNormalListOrDevList(items);

        int different_item_types = 0;
        Dictionary<Item, int> amount_of_each_item = new Dictionary<Item, int>();

        foreach (Item item in items_to_use)
        {
            Array<Item> i_list = PlayerInventoryUI.instance?.GetItemFromListOrNull(
                PlayerInventoryUI.instance?.GetListOfItemsInInventory(),
                item
            );

            int amount_of_item = 0;
            if (i_list == null)
                continue;

            foreach (Item i in i_list)
                amount_of_item += i.amount;

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

    public Array<Item> GetNormalListOrDevList(Array<Item> items)
    {
        if (GameManager.dev_build_mode)
            return new Array<Item>()
            {
                new Item(Inventory.ITEM_TYPES[Inventory.ITEM_ID.OAK_WOOD], 1)
            };
        return items;
    }

    private void SetTimesToBuildLabel(int times)
    {
        PlayerUI.instance?.times_to_build_left_label?.Set(
            "text",
            $"> {times}x {TranslationServer.Translate("PLAYERUI_TIMES_LEFT_TO_BUILD")}"
        );
    }

    private void ClearChildren()
    {
        foreach (Control c in GetChildren())
            c.QueueFree();
    }

    private h_box_item CreateHBoxItem(Item item)
    {
        h_box_item hbi = (h_box_item)h_box_item.Instantiate();
        hbi.InitItemUI("", item.amount, item.info.texture);
        hbi.ChangeColor(global::h_box_item.colorType.red);
        return hbi;
    }
}
