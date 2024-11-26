using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class InventoryBase : Control
{
    public Array<Slot> slots = new Array<Slot>();

    [Export]
    public ItemSave[] inventory_items = new ItemSave[20];
    public Dictionary<ITEM_ID, ItemInfo> item_Types = new Dictionary<ITEM_ID, ItemInfo>()
    {
        { ITEM_ID.WOOD, ResourceLoader.Load<ItemInfo>("res://Items/Wood.tres") },
        { ITEM_ID.STONE, ResourceLoader.Load<ItemInfo>("res://Items/Stone.tres") },
        { ITEM_ID.WOOD_STICK, ResourceLoader.Load<ItemInfo>("res://Items/Wood_Stick.tres") },
        { ITEM_ID.STONE_PICKAXE, ResourceLoader.Load<ItemInfo>("res://Items/Stone_Pickaxe.tres") },
        { ITEM_ID.WOODEN_PLANK, ResourceLoader.Load<ItemInfo>("res://Items/Wood_Plank.tres") },
        { ITEM_ID.Wooden_Axe, ResourceLoader.Load<ItemInfo>("res://Items/Wooden_Axe.tres") },
        { ITEM_ID.CHAR_COAL, ResourceLoader.Load<ItemInfo>("res://Items/Char_Coal.tres") },
        { ITEM_ID.IRON_ORE, ResourceLoader.Load<ItemInfo>("res://Items/Iron_Ore.tres") },
        { ITEM_ID.IRON_INGOT, ResourceLoader.Load<ItemInfo>("res://Items/Iron_Ingot.tres") },
        { ITEM_ID.COPPER_ORE, ResourceLoader.Load<ItemInfo>("res://Items/Copper_Ore.tres") },
        { ITEM_ID.COPPER_INGOT, ResourceLoader.Load<ItemInfo>("res://Items/Copper_Ingot.tres") },
        { ITEM_ID.STONE_KNIFE, ResourceLoader.Load<ItemInfo>("res://Items/Stone_Knife.tres") },
        { ITEM_ID.MYSTIC_FIBRE, ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Fibre.tres") },
        { ITEM_ID.MYSTIC_WOOD, ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Wood.tres") },
        { ITEM_ID.FIBRE, ResourceLoader.Load<ItemInfo>("res://Items/Fibre.tres") },
        { ITEM_ID.STONE_BLADE, ResourceLoader.Load<ItemInfo>("res://Items/Stone_Blade.tres") },
        { ITEM_ID.GLASS_CHUNK, ResourceLoader.Load<ItemInfo>("res://Items/Glass_Chunk.tres") },
        {
            ITEM_ID.STONE_AXE_HEAD,
            ResourceLoader.Load<ItemInfo>("res://Items/Stone_Axe_Head.tres")
        },
        {
            ITEM_ID.STONE_PICKAXE_HEAD,
            ResourceLoader.Load<ItemInfo>("res://Items/Stone_Pickaxe_Head.tres")
        },
        {
            ITEM_ID.WOODEN_HOE_HEAD,
            ResourceLoader.Load<ItemInfo>("res://Items/Wooden_Hoe_Head.tres")
        },
        { ITEM_ID.WOODEN_HOE, ResourceLoader.Load<ItemInfo>("res://Items/Wooden_Hoe.tres") },
        {
            ITEM_ID.WOODEN_AXE_HEAD,
            ResourceLoader.Load<ItemInfo>("res://Items/Wooden_Axe_Head.tres")
        },
        { ITEM_ID.WHEAT, ResourceLoader.Load<ItemInfo>("res://Items/Wheat.tres") },
        { ITEM_ID.IRON_BLOCK, ResourceLoader.Load<ItemInfo>("res://Items/Iron_Block.tres") },
        { ITEM_ID.WHEAT_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Wheat_Seed.tres") },
        { ITEM_ID.POTATO, ResourceLoader.Load<ItemInfo>("res://Items/Potato.tres") },
        { ITEM_ID.POTATO_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Potato_Seed.tres") },
        { ITEM_ID.CARROT, ResourceLoader.Load<ItemInfo>("res://Items/Carrot.tres") },
        { ITEM_ID.CARROT_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Carrot_Seed.tres") },
        { ITEM_ID.CORN, ResourceLoader.Load<ItemInfo>("res://Items/Corn.tres") },
        { ITEM_ID.CORN_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Corn_Seed.tres") },
        { ITEM_ID.SAND, ResourceLoader.Load<ItemInfo>("res://Items/Sand.tres") },
        { ITEM_ID.SAND_STONE, ResourceLoader.Load<ItemInfo>("res://Items/Sand_Stone.tres") },
        {
            ITEM_ID.MYSTIC_ARMOR_HEAD,
            ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Armor_Helm.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_CHESTPLATE,
            ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Armor_Chestplate.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_LEGGINGS,
            ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Armor_Leggings.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_SHOES,
            ResourceLoader.Load<ItemInfo>("res://Items/Mystic_Armor_Shoes.tres")
        },
    };

    public enum ITEM_ID
    {
        WOOD,
        STONE,
        WOOD_STICK,
        STONE_PICKAXE,
        WOODEN_PLANK,
        Wooden_Axe,
        CHAR_COAL,
        IRON_ORE,
        IRON_INGOT,
        COPPER_ORE,
        COPPER_INGOT,
        STONE_KNIFE,
        MYSTIC_FIBRE,
        MYSTIC_WOOD,
        FIBRE,
        STONE_BLADE,
        GLASS_CHUNK,
        STONE_AXE_HEAD,
        STONE_PICKAXE_HEAD,
        WOODEN_HOE_HEAD,
        WOODEN_HOE,
        WOODEN_AXE_HEAD,
        WHEAT,
        IRON_BLOCK,
        WHEAT_SEED,
        POTATO,
        POTATO_SEED,
        CARROT,
        CARROT_SEED,
        CORN,
        CORN_SEED,
        SAND,
        SAND_STONE,
        MYSTIC_ARMOR_HEAD,
        MYSTIC_ARMOR_CHESTPLATE,
        MYSTIC_ARMOR_LEGGINGS,
        MYSTIC_ARMOR_SHOES
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
        if (item_to_find == null)
            return null;

        foreach (Item i in itemsInInventory)
            if (i.item_info.item_name.Equals(item_to_find.item_info.item_name))
                return i;

        return null;
    }

    public Array<Item> GetListOfItemsInInventory()
    {
        Array<Item> items = new Array<Item>();
        foreach (ItemSave s in inventory_items)
        {
            if (s != null)
            {
                Item item = new Item
                {
                    item_info = item_Types[((ITEM_ID)s.item_id)],
                    amount = s.amount
                };
                items.Add(item);
            }
        }
        return items;
    }

    public void AddItem(ItemInfo ii, int amount, ItemSave[] array)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)ii.unique_id)
            {
                if (array[i].amount + amount <= 0)
                    array[i] = null;
                else
                    array[i].amount += amount;

                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );
                UpdateSlotUI(i);
                return;
            }
        }

        //Check latest Slot which is Null
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
            {
                array[i] = new ItemSave((int)ii.unique_id, amount);
                QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
                    QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
                );

                UpdateSlotUI(i);
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
            if (array[i].item_id == (int)bi.item.item_info.unique_id)
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
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory_items.Length; i++)
            UpdateSlotUI(i);
    }

    public void UpdateSlotUI(int i)
    {
        GetNode<Slot>($"GridContainer/Slot{i}").ClearItem();

        if (inventory_items[i] == null)
            return;

        GetNode<Slot>($"GridContainer/Slot{i}")
            .SetItem(item_Types[(ITEM_ID)inventory_items[i].item_id], inventory_items[i].amount);

        QuestMiniPanel.INSTANCE.UpdateQuestMiniPanel(
            QuestManager.INSTANCE.quests[QuestManager.current_quest_id]
        );
    }

    public void LoadInventoryFromSave(ItemSave[] item_save)
    {
        inventory_items = new ItemSave[20];
        inventory_items = item_save;
        UpdateInventoryUI();
    }
}
