using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class InventoryBase : Control
{
    public Array<Slot> slots = new Array<Slot>();

    [Export]
    public ItemSave[] inventory_items = new ItemSave[20];
    public ItemInfo[] item_Types =
    {
        ResourceLoader.Load<ItemInfo>("res://Items/Wood.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Stone.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Wood_Stick.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Stone_Pickaxe.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Wood_Plank.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Wood_Chest.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Char_Coal.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Iron_Ore.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Iron_Ingot.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Copper_Ore.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Copper_Ingot.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Stone_Knife.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Fibre.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Wood.tres"),
    };

    public override void _Ready()
    {
        for (int i = 0; i < item_Types.Length; i++)
        {
            for (int j = 0; j < item_Types.Length; j++)
                if (item_Types[i].unique_item_id == item_Types[j].unique_item_id && i != j)
                {
                    GD.PrintErr(
                        "UNIQUE ITEM ID IS DUPLICATED: i: "
                            + item_Types[i].item_name
                            + "j: "
                            + item_Types[j].item_name
                    );
                    return;
                }
        }
    }

    public void SetSlots()
    {
        foreach (Slot s in GetNode<GridContainer>($"GridContainer").GetChildren())
            slots.Add(s);
    }

    public Slot FindSlotWithItemInInventory(ItemInfo item_info)
    {
        foreach (Slot s in slots)
        {
            InventoryItem i = s.GetItem();
            if (i != null)
                if (i.item_info == item_info)
                    return s;
        }
        return null;
    }

    public Item GetItemFromList(Array<Item> itemsInInventory, Item item_to_find)
    {
        foreach (Item i in itemsInInventory)
            if (i.item_info.item_name.Equals(item_to_find.item_info.item_name))
                return i;

        return null;
    }

    public Array<Item> GetListOfItemsInInventory()
    {
        Array<Item> items = new Array<Item>();
        foreach (Slot s in slots)
        {
            InventoryItem i = s.GetItem();
            if (i != null)
            {
                Item item = new Item { item_info = i.item_info, amount = i.amount };
                items.Add(item);
            }
        }
        return items;
    }

    public void AddItem(ItemInfo item_info, int amount, ItemSave[] array)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == item_info.unique_item_id)
            {
                array[i].amount += amount;

                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );
                UpdateInventoryUI();
                return;
            }
        }

        //Check latest Slot which is Null
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
            {
                array[i] = new ItemSave(item_info.unique_item_id, amount);
                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );

                UpdateInventoryUI();
                return;
            }
    }

    public bool HasItemInInventory(ItemSave[] array, BeltItem bi)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (bi == null)
            {
                Debug.Print("BI IS NULL");
                return false;
            }
            if (array[i].item_id == bi.item.item_info.unique_item_id)
                return true;
        }
        return false;
    }

    public bool HasEmptySlotInInventory(ItemSave[] array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
                return true;

        return false;
    }

    public void OnVisiblityChange()
    {
        UpdateInventoryUI();
        Debug.Print("Updated Invs");
    }

    public void UpdateInventoryUI()
    {
        ClearInventory();

        for (int i = 0; i < inventory_items.Length; i++)
        {
            if (inventory_items[i] != null)
                GetNode<Slot>($"GridContainer/Slot{i}")
                    .SetItem(item_Types[inventory_items[i].item_id], inventory_items[i].amount);
        }
    }

    public void LoadInventoryFromSave(ItemSave[] item_save)
    {
        inventory_items = new ItemSave[20];
        inventory_items = item_save;
        UpdateInventoryUI();
    }

    public void ClearInventory()
    {
        foreach (Slot s in slots)
            s.ClearItem();
    }
}
