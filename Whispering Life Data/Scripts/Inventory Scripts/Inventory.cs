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
    public static string item_path = "res://Resource Meta/Items/item_info_";
    public static Dictionary<ITEM_ID, ItemInfo> ITEM_TYPES = new Dictionary<ITEM_ID, ItemInfo>()
    {
        { ITEM_ID.OAK_WOOD, ResourceLoader.Load<ItemInfo>(item_path + "wood.tres") },
        { ITEM_ID.STONE, ResourceLoader.Load<ItemInfo>(item_path + "stone.tres") },
        { ITEM_ID.WOOD_STICK, ResourceLoader.Load<ItemInfo>(item_path + "wood_stick.tres") },
        { ITEM_ID.STONE_PICKAXE, ResourceLoader.Load<ItemInfo>(item_path + "stone_pickaxe.tres") },
        { ITEM_ID.WOOD_PLANK, ResourceLoader.Load<ItemInfo>(item_path + "wood_plank.tres") },
        { ITEM_ID.WOOD_AXE, ResourceLoader.Load<ItemInfo>(item_path + "wood_axe.tres") },
        { ITEM_ID.CHAR_COAL, ResourceLoader.Load<ItemInfo>(item_path + "char_coal.tres") },
        { ITEM_ID.IRON_ORE, ResourceLoader.Load<ItemInfo>(item_path + "iron_ore.tres") },
        { ITEM_ID.IRON_INGOT, ResourceLoader.Load<ItemInfo>(item_path + "iron_ingot.tres") },
        { ITEM_ID.COPPER_ORE, ResourceLoader.Load<ItemInfo>(item_path + "copper_ore.tres") },
        { ITEM_ID.COPPER_INGOT, ResourceLoader.Load<ItemInfo>(item_path + "copper_ingot.tres") },
        { ITEM_ID.STONE_KNIFE, ResourceLoader.Load<ItemInfo>(item_path + "stone_knife.tres") },
        { ITEM_ID.MYSTIC_FIBRE, ResourceLoader.Load<ItemInfo>(item_path + "mystic_fibre.tres") },
        {
            ITEM_ID.MYSTIC_OAK_WOOD,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_oak_wood.tres")
        },
        { ITEM_ID.FIBRE, ResourceLoader.Load<ItemInfo>(item_path + "fibre.tres") },
        { ITEM_ID.STONE_BLADE, ResourceLoader.Load<ItemInfo>(item_path + "stone_blade.tres") },
        { ITEM_ID.GLASS_CHUNK, ResourceLoader.Load<ItemInfo>(item_path + "glass_chunk.tres") },
        {
            ITEM_ID.STONE_AXE_HEAD,
            ResourceLoader.Load<ItemInfo>(item_path + "stone_axe_head.tres")
        },
        {
            ITEM_ID.STONE_PICKAXE_HEAD,
            ResourceLoader.Load<ItemInfo>(item_path + "stone_pickaxe_head.tres")
        },
        { ITEM_ID.WOOD_HOE_HEAD, ResourceLoader.Load<ItemInfo>(item_path + "wood_hoe_head.tres") },
        { ITEM_ID.WOOD_HOE, ResourceLoader.Load<ItemInfo>(item_path + "wood_hoe.tres") },
        { ITEM_ID.WOOD_AXE_HEAD, ResourceLoader.Load<ItemInfo>(item_path + "wood_axe_head.tres") },
        { ITEM_ID.WHEAT, ResourceLoader.Load<ItemInfo>(item_path + "wheat.tres") },
        { ITEM_ID.IRON_BLOCK, ResourceLoader.Load<ItemInfo>(item_path + "iron_block.tres") },
        { ITEM_ID.WHEAT_SEED, ResourceLoader.Load<ItemInfo>(item_path + "wheat_seed.tres") },
        { ITEM_ID.POTATO, ResourceLoader.Load<ItemInfo>(item_path + "potato.tres") },
        { ITEM_ID.POTATO_SEED, ResourceLoader.Load<ItemInfo>(item_path + "potato_seed.tres") },
        { ITEM_ID.CARROT, ResourceLoader.Load<ItemInfo>(item_path + "carrot.tres") },
        { ITEM_ID.CARROT_SEED, ResourceLoader.Load<ItemInfo>(item_path + "carrot_seed.tres") },
        { ITEM_ID.CORN, ResourceLoader.Load<ItemInfo>(item_path + "corn.tres") },
        { ITEM_ID.CORN_SEED, ResourceLoader.Load<ItemInfo>(item_path + "corn_seed.tres") },
        { ITEM_ID.SAND, ResourceLoader.Load<ItemInfo>(item_path + "sand.tres") },
        { ITEM_ID.SAND_STONE, ResourceLoader.Load<ItemInfo>(item_path + "sand_stone.tres") },
        {
            ITEM_ID.MYSTIC_ARMOR_HEAD,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_armor_head.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_CHESTPLATE,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_armor_chestplate.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_LEGGINGS,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_armor_leggings.tres")
        },
        {
            ITEM_ID.MYSTIC_ARMOR_SHOES,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_armor_shoes.tres")
        },
        { ITEM_ID.OAK_SEED, ResourceLoader.Load<ItemInfo>(item_path + "oak_seed.tres") },
        {
            ITEM_ID.MYSTIC_OAK_SEED,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_oak_seed.tres")
        },
        {
            ITEM_ID.MYSTIC_FIBRE_SEED,
            ResourceLoader.Load<ItemInfo>(item_path + "mystic_fibre_seed.tres")
        },
    };

    public enum ITEM_ID
    {
        OAK_WOOD,
        STONE,
        WOOD_STICK,
        STONE_PICKAXE,
        WOOD_PLANK,
        WOOD_AXE,
        CHAR_COAL,
        IRON_ORE,
        IRON_INGOT,
        COPPER_ORE,
        COPPER_INGOT,
        STONE_KNIFE,
        MYSTIC_FIBRE,
        MYSTIC_OAK_WOOD,
        FIBRE,
        STONE_BLADE,
        GLASS_CHUNK,
        STONE_AXE_HEAD,
        STONE_PICKAXE_HEAD,
        WOOD_HOE_HEAD,
        WOOD_HOE,
        WOOD_AXE_HEAD,
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
        MYSTIC_OAK_SEED,
        MYSTIC_FIBRE_SEED
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
            SlotItemUI slot_item = slot.GetSlotItemUI();
            if (slot_item != null)
                if (slot_item.item.info == item.info)
                    return slot;
        }
        return null;
    }

    public void MarkSlotsWithAttributeTypes(Type[] attributeTypes)
    {
        if (attributeTypes == null || attributeTypes.Length == 0)
        {
            Debug.Print("No attribute types provided for marking slots.");
            return;
        }

        Debug.Print("Updating Slots with markings for multiple attribute types");
        foreach (Slot slot in slots)
        {
            if (slot.GetSlotItemUI() != null)
            {
                slot.GetSlotItemUI().SelfModulate = new Color(1f, 1f, 1f);

                if (ItemHasZeroAttributeTypes(attributeTypes, slot.GetSlotItemUI()))
                    slot.GetSlotItemUI().SelfModulate = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private bool ItemHasZeroAttributeTypes(Type[] attributeTypes, SlotItemUI slot_item_ui)
    {
        if (attributeTypes == null)
            return false;

        foreach (Type type in attributeTypes)
        {
            if (slot_item_ui.item.info.HasAttributByType(type))
                return false;
        }
        return true;
    }

    public Array<Item> GetItemFromList(Array<Item> itemsInInventory, Item item_to_find)
    {
        if (item_to_find == null)
            return null;
        Array<Item> items = new Array<Item>();

        foreach (Item i in itemsInInventory)
            if (i.info.name.Equals(item_to_find.info.name))
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
                Item item = new Item { info = ITEM_TYPES[(ITEM_ID)s.item_id], amount = s.amount };
                items.Add(item);
            }
        }
        return items;
    }

    public void AddItem(Item item, ItemSave[] array)
    {
        ItemInfo item_info = item.info;
        //Check if Item already exists // 70
        int remaining = item.amount;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (array[i].item_id == (int)item_info.id)
            {
                // 20 + 70
                if (remaining > 0)
                {
                    if (array[i].amount == item_info.max_stackable_size)
                        continue;
                    // 47 + 70 != 48
                    if (array[i].amount + remaining <= item_info.max_stackable_size)
                    {
                        array[i].amount += remaining;
                        remaining = 0;
                        UpdateSlot(i);
                        return;
                    }
                    else
                    { // 48 - 47 = 1
                        int diff = item_info.max_stackable_size - array[i].amount;
                        array[i].amount = item_info.max_stackable_size;
                        remaining -= diff;
                        UpdateSlot(i);
                    }
                }
            }

            QuestMiniPanel.instance.UpdateQuestMiniPanel(
                QuestManager.instance.quests[QuestManager.current_quest_id]
            );
        }

        ToolAttribute attribute = item_info.GetAttributeOrNull<ToolAttribute>();
        //Check latest Slot which is Null
        for (int i = 0; i < array.Length; i++)
            if (array[i] == null)
            {
                if (remaining <= item_info.max_stackable_size)
                {
                    if (attribute != null)
                        array[i] = new ItemSave((int)item_info.id, remaining, attribute.durability);
                    else
                        array[i] = new ItemSave((int)item_info.id, remaining);

                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    UpdateSlot(i);
                    return;
                }
                else
                {
                    if (attribute != null)
                        array[i] = new ItemSave((int)item_info.id, remaining, attribute.durability);
                    else
                        array[i] = new ItemSave((int)item_info.id, item_info.max_stackable_size);
                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    remaining -= item_info.max_stackable_size;
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

            if (array[i].item_id == (int)item.info.id)
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

            if (array[i].item_id == (int)item.info.id)
            {
                if (array[i].amount + item.amount < item.info.max_stackable_size)
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

            if (array[i].item_id == (int)item.info.id)
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
            if (array[i].item_id == (int)belt_item.item.info.id)
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

    public override void UpdateSlot(int index, SlotItemUI pref_ref = null)
    {
        if (inventory_items[index] == null)
        {
            ClearSlot(index);
            return;
        }

        GetNode<Slot>($"GridContainer/Slot{index}")
            .UpdateItem(
                new Item(GetItemInfo(index), inventory_items[index].amount),
                GetDurability(index)
            );

        if (pref_ref != null)
            GetNode<Slot>($"GridContainer/Slot{index}").GetSlotItemUI().SelfModulate =
                pref_ref.SelfModulate;

        QuestMiniPanel.instance.UpdateQuestMiniPanel(
            QuestManager.instance.quests[QuestManager.current_quest_id]
        );
    }

    public override void ClearSlot(int index)
    {
        GetNode<Slot>($"GridContainer/Slot{index}").ClearSlotItem();
    }

    //TODO: GetItemInfo ziemlich oft vertreten! Refactorn!
    public override ItemInfo GetItemInfo(int index)
    {
        return ITEM_TYPES[(ITEM_ID)inventory_items[index].item_id];
    }

    public int GetDurability(int index)
    {
        ToolAttribute attribute = GetItemInfo(index).GetAttributeOrNull<ToolAttribute>();
        if (attribute != null)
            return inventory_items[index].current_durability;
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
