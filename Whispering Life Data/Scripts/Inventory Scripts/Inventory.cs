using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Inventory : SlotUpdater
{
    public Array<Slot> slots = new Array<Slot>();

    [Export]
    public ItemSave[] inventory_items;

    public int slot_amount = 30;
    public static Dictionary<ITEM_ID, ItemInfo> ITEM_TYPES = new Dictionary<ITEM_ID, ItemInfo>()
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
        { ITEM_ID.OAK_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Oak_Seed.tres") },
        { ITEM_ID.MYST_OAK_SEED, ResourceLoader.Load<ItemInfo>("res://Items/Myst_Oak_Seed.tres") },
        {
            ITEM_ID.MYST_FIBRE_SEED,
            ResourceLoader.Load<ItemInfo>("res://Items/Myst_Fibre_Seed.tres")
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
        MYSTIC_ARMOR_SHOES,
        OAK_SEED,
        MYST_OAK_SEED,
        MYST_FIBRE_SEED
    }

    public override void _Ready()
    {
        inventory_items = new ItemSave[slot_amount];
    }

    public void SetSlots()
    {
        foreach (Slot s in GetNode<GridContainer>($"GridContainer").GetChildren())
            slots.Add(s);
    }

    public Slot GetSlotWithItem(Item item)
    {
        foreach (Slot slot in slots)
        {
            SlotItem slot_item = slot.GetSlotItem();
            if (slot_item != null)
                if (slot_item.item_info == item.item_info)
                    return slot;
        }
        return null;
    }

    public void MarkSlotsWithType(ItemInfo.Type[] types)
    {
        Debug.Print("Updating Slots wirh markings");
        foreach (Slot slot in slots)
        {
            if (slot.GetSlotItem() != null)
            {
                slot.GetSlotItem().SelfModulate = new Color(1f, 1f, 1f);

                if (ItemHasZeroTypes(types, slot.GetSlotItem().item_info))
                    slot.GetSlotItem().SelfModulate = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private bool ItemHasZeroTypes(ItemInfo.Type[] types, ItemInfo ii)
    {
        if (types == null)
            return false;

        foreach (ItemInfo.Type type in types)
            if (!ii.HasType(type))
                continue;
            else
                return false;
        return true;
    }

    public Array<Item> GetItemFromList(Array<Item> itemsInInventory, Item item_to_find)
    {
        if (item_to_find == null)
            return null;
        Array<Item> items = new Array<Item>();

        foreach (Item i in itemsInInventory)
            if (i.item_info.item_name.Equals(item_to_find.item_info.item_name))
                items.Add(i);

        if (items.Count == 0)
            return null;

        return items;
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
                    item_info = ITEM_TYPES[(ITEM_ID)s.item_id],
                    amount = s.amount
                };
                items.Add(item);
            }
        }
        return items;
    }

    public void AddItem(Item item, ItemSave[] array)
    {
        ItemInfo item_info = item.item_info;
        //Check if Item already exists // 70
        int remaining = item.amount;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)item_info.item_id)
            {
                // 20 + 70
                if (remaining > 0)
                {
                    if (array[i].amount == item_info.max_slot_amount)
                        continue;
                    // 47 + 70 != 48
                    if (array[i].amount + remaining <= item_info.max_slot_amount)
                    {
                        array[i].amount += remaining;
                        remaining = 0;
                        UpdateSlot(i);
                        return;
                    }
                    else
                    { // 48 - 47 = 1
                        int diff = item_info.max_slot_amount - array[i].amount;
                        array[i].amount = item_info.max_slot_amount;
                        remaining -= diff;
                        UpdateSlot(i);
                    }
                }
            }

            QuestMiniPanel.instance.UpdateQuestMiniPanel(
                QuestManager.instance.quests[QuestManager.current_quest_id]
            );
        }

        //Check latest Slot which is Null
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
            {
                if (remaining <= item_info.max_slot_amount)
                {
                    if (item_info.has_durability)
                        array[i] = new ItemSave(
                            (int)item_info.item_id,
                            remaining,
                            item_info.max_durability
                        );
                    else
                        array[i] = new ItemSave((int)item_info.item_id, remaining);

                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    UpdateSlot(i);
                    return;
                }
                else
                {
                    if (item_info.has_durability)
                        array[i] = new ItemSave(
                            (int)item_info.item_id,
                            remaining,
                            item_info.max_durability
                        );
                    else
                        array[i] = new ItemSave((int)item_info.item_id, item_info.max_slot_amount);
                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    remaining -= item_info.max_slot_amount;
                }
            }
    }

    public void RemoveItem(Item item, ItemSave[] array)
    {
        //Check if Item already exists
        int remaining = item.amount; //10
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)item.item_info.item_id)
            {
                // 4 - 6 = -2
                if ((array[i].amount - remaining) > 0)
                {
                    array[i].amount -= remaining;
                    remaining = 0;
                    UpdateSlot(i);
                }
                else
                {
                    remaining -= array[i].amount;
                    array[i] = null;
                    UpdateSlot(i);
                }
            }
        }
        if (remaining < 0)
            GD.PrintErr("Not all resources got removed, cause not enough items!");

        QuestMiniPanel.instance.UpdateQuestMiniPanel(
            QuestManager.instance.quests[QuestManager.current_quest_id]
        );
    }

    public bool CanReceiveItem(Item item, ItemSave[] array)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)item.item_info.item_id)
            {
                if (array[i].amount + item.amount < item.item_info.max_slot_amount)
                    return true;
            }
        }

        //Check latest Slot which is Null
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
                return true;

        return false;
    }

    public bool HasEnoughResources(Item item, ItemSave[] array)
    {
        int item_amount = 0;
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)item.item_info.item_id)
            {
                item_amount += array[i].amount;

                if (item_amount >= item.amount)
                    return true;
            }
        }
        return false;
    }

    public bool HasItemInInventory(ItemSave[] array, BeltItem belt_item)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (belt_item == null)
            {
                Debug.Print("BI IS NULL");
                return false;
            }
            if (array[i].item_id == (int)belt_item.item.item_info.item_id)
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
            UpdateSlot(i);
    }

    public override void UpdateSlot(int index, SlotItem pref_ref = null)
    {
        if (inventory_items[index] == null)
        {
            ClearSlot(index);
            return;
        }

        GetNode<Slot>($"GridContainer/Slot{index}")
            .UpdateItem(GetItemInfo(index), inventory_items[index].amount, GetDurability(index));

        if (pref_ref != null)
            GetNode<Slot>($"GridContainer/Slot{index}").GetSlotItem().SelfModulate =
                pref_ref.SelfModulate;

        QuestMiniPanel.instance.UpdateQuestMiniPanel(
            QuestManager.instance.quests[QuestManager.current_quest_id]
        );
    }

    public override void ClearSlot(int index)
    {
        GetNode<Slot>($"GridContainer/Slot{index}").ClearItem();
    }

    public override ItemInfo GetItemInfo(int index)
    {
        return ITEM_TYPES[(ITEM_ID)inventory_items[index].item_id];
    }

    public int GetDurability(int index)
    {
        Debug.Print("" + GetItemInfo(index).has_durability);
        if (GetItemInfo(index).has_durability)
        {
            Debug.Print(inventory_items[index].current_durability.ToString());
            return inventory_items[index].current_durability;
        }
        else
            return -1;
    }

    public void LoadInventoryFromSave(ItemSave[] item_save)
    {
        inventory_items = new ItemSave[slot_amount];
        inventory_items = item_save;
        UpdateInventoryUI();
    }
}
