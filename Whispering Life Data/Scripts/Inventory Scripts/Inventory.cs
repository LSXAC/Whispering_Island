using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Inventory : Control
{
    [Export]
    public int MaxSlots = 20;

    public static CharacterSave char_save = new CharacterSave();
    private ItemInfo[] item_Types =
    {
        ResourceLoader.Load<ItemInfo>("res://Items/Wood.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Stone.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Sticks.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Pickaxe.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Plank.tres"),
        ResourceLoader.Load<ItemInfo>("res://Items/Chest.tres"),
    };
    public static Inventory INSTANCE = null;

    public Array<Slot> slots = new Array<Slot>();

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
        foreach (Slot s in GetNode<GridContainer>($"GridContainer").GetChildren())
            slots.Add(s);

        INSTANCE = this;

        //Instantiate new Items
    }

    public static Slot FindSlotWithItemInInventory(ItemInfo item_info)
    {
        if (INSTANCE != null)
            foreach (Slot s in INSTANCE.slots)
            {
                InventoryItem i = s.GetItem();
                if (i != null)
                    if (i.item_info == item_info)
                        return s;
            }
        return null;
    }

    public static Item GetItemFromList(Array<Item> itemsInInventory, Item item_to_find)
    {
        foreach (Item i in itemsInInventory)
            if (i.item_info.item_name.Equals(item_to_find.item_info.item_name))
                return i;

        return null;
    }

    public static Array<Item> GetListOfItemsInInventory()
    {
        Array<Item> items = new Array<Item>();
        if (INSTANCE != null)
            foreach (Slot s in INSTANCE.slots)
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

    public static void LoadInventory()
    {
        foreach (Slot s in INSTANCE.slots)
            s.ClearItem();

        for (int i = 0; i < char_save.inventory_items.Count; i++)
        {
            Debug.Print("LOADING");
            InventoryItem ii = new InventoryItem();
            ii.init(INSTANCE.item_Types[char_save.inventory_items[i].item_id]);

            INSTANCE
                .GetNode<Slot>($"GridContainer/Slot{char_save.inventory_items[i].slot_id}")
                .SetItem(ii, char_save.inventory_items[i].amount);
        }

        //InventoryItem iix = new InventoryItem();
        //iix.init(INSTANCE.item_Types[1]);
        //INSTANCE.GetNode<Slot>($"GridContainer/Slot{10}").SetItem(iix, 1);
    }

    public static void SaveInventory()
    {
        char_save.inventory_items = new Array<ItemSave>();
        for (int i = 0; i < INSTANCE.slots.Count; i++)
        {
            Slot s = INSTANCE.slots[i];
            if (s.GetItem() != null)
                char_save.inventory_items.Add(
                    new ItemSave(s.GetItem().item_info.unique_item_id, s.GetItem().amount, i)
                );
        }
    }

    public void AddItem(ItemInfo item_info, int amount)
    {
        //Check if Item already exists
        for (int i = 0; i < INSTANCE.slots.Count; i++)
        {
            if (INSTANCE.slots[i].GetItem() == null)
                continue;
            if (INSTANCE.slots[i].GetItem().item_info == item_info)
            {
                INSTANCE.slots[i].UpdateItem(amount);
                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );
                return;
            }
        }

        //Check latest Slot which is Null
        for (int i = 0; i < INSTANCE.slots.Count; i++)
            if (INSTANCE.slots[i].GetItem() == null)
            {
                INSTANCE
                    .slots[i]
                    .SetItem(CreateInventoryItem(item_Types[item_info.unique_item_id]), amount);
                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );
                return;
            }
    }

    public InventoryItem CreateInventoryItem(ItemInfo item_info)
    {
        InventoryItem ii = new InventoryItem();
        ii.init(item_info);
        return ii;
    }
}
