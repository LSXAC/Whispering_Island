using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Inventory : SlotUpdater
{
    [Export]
    public Array<Slot> slots = new Array<Slot>();

    [Export]
    public ItemSave[] inventory_items;

    public int slot_amount = 30;
    public static string item_path = "res://Resource Meta/Items/item_info_";
    public static Dictionary<ITEM_ID, ItemInfo> ITEM_TYPES = new Dictionary<ITEM_ID, ItemInfo>();

    public enum ITEM_ID
    {
        NULL,
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
        SUNFLOWER,
        SUNFLOWER_SEED,
        CARROT,
        CARROT_SEED,
        MUSHROOM,
        MUSHROOM_SPORES,
        SAND,
        SAND_STONE,
        MYSTIC_ARMOR_HEAD,
        MYSTIC_ARMOR_CHESTPLATE,
        MYSTIC_ARMOR_LEGGINGS,
        MYSTIC_ARMOR_SHOES,
        OAK_SEED,
        MYSTIC_OAK_SEED,
        MYSTIC_FIBRE_SEED,
        FIBRE_SEED,
        OAK_WOOD,
        GEAR,
        MACHINE_COMPONENT,
        TRANSPORT_COMPONENT,
        COAL,
        SMELT_COMPONENT,
        GROWTH_COMPONENT,
        SOIL,
        RESEARCH_COMPONENT,
        LIGHT_COMPONENT,
        INVENTORY_COMPONENT,
        DESTROY_COMPONENT,
        SLEEP_COMPONENT,
        SUGAR_CANE,
        SUGAR_CANE_SEED,
        SUGAR,
        RESEARCH_BOOK_SUB,
        RESEARCH_BOOK_LEVEL,
        PAPER,
        MYSTIC_MUSHROOM,
        MYSTIC_MUSHROOM_SPORES,
        PALM_WOOD,
        PALM_SEED,
        CACTUS,
        CACTUS_SEED,
        MAGIC_DUST,
        MAGIC_CORE_COMPONENT
    }

    public Action OnItemChanged;

    public override void _Ready()
    {
        inventory_items = new ItemSave[slot_amount];
    }

    public static string GetToolTipItem(Item item)
    {
        string result = "";
        if (item?.info == null)
            return "";
        result = TranslationServer.Translate(item.info.name.ToString()) + "\n";
        result += TranslationServer.Translate(item.info.description.ToString()) + "\n";
        result += "\n";
        foreach (ItemAttributeBase item_attribute in item.info.attributes)
        {
            if (item_attribute == null)
                continue;

            result += item_attribute.GetNameOfAttribute();
        }
        return result;
    }

    public void SetSlots()
    {
        foreach (ColorRect rect in GetNode<GridContainer>($"GridContainer").GetChildren())
            slots.Add(rect.GetChild<Slot>(0));
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

    public void AddDurabilityToItemsThroughAutoRepair()
    {
        for (int i = 0; i < inventory_items.Length; i++)
        {
            if (inventory_items[i] == null)
                continue;

            if (inventory_items[i].current_durability == -1)
                continue;

            if (
                ITEM_TYPES[(ITEM_ID)inventory_items[i].item_id].GetAttributeOrNull<ToolAttribute>()
                == null
            )
                continue;

            if (
                inventory_items[i].current_durability
                < ITEM_TYPES[(ITEM_ID)inventory_items[i].item_id]
                    .GetAttributeOrNull<ToolAttribute>()
                    .durability
                    * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.TOOL_DURABILITY)
            )
                inventory_items[i].current_durability += 1;

            UpdateSlot(i);
        }

        ItemSave[] equip = EquipmentPanel.instance.equipped_tools;
        for (int i = 0; i < equip.Length; i++)
        {
            if (equip[i] == null)
                continue;

            if (equip[i].current_durability == -1)
                continue;

            if (ITEM_TYPES[(ITEM_ID)equip[i].item_id].GetAttributeOrNull<ToolAttribute>() == null)
                continue;

            if (
                equip[i].current_durability
                < ITEM_TYPES[(ITEM_ID)equip[i].item_id]
                    .GetAttributeOrNull<ToolAttribute>()
                    .durability
                    * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.TOOL_DURABILITY)
            )
                equip[i].current_durability += 1;

            EquipmentPanel.UpdateSlots();
        }
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

    public Array<Item> GetItemFromListOrNull(Array<Item> itemsInInventory, Item item_to_find)
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

    public void AddItem(Item item, ItemSave[] inventory)
    {
        ItemInfo item_info = item.info;
        //Check if Item already exists // 70
        int remaining = item.amount;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
                continue;

            if (inventory[i].item_id == (int)item_info.id && inventory[i].state == (int)item.state)
            {
                // 20 + 70
                if (remaining > 0)
                {
                    if (inventory[i].amount == item_info.max_stackable_size)
                        continue;
                    // 47 + 70 != 48
                    if (inventory[i].amount + remaining <= item_info.max_stackable_size)
                    {
                        inventory[i].amount += remaining;
                        remaining = 0;
                        UpdateSlot(i);
                        return;
                    }
                    else
                    { // 48 - 47 = 1
                        int diff = item_info.max_stackable_size - inventory[i].amount;
                        inventory[i].amount = item_info.max_stackable_size;
                        remaining -= diff;
                        UpdateSlot(i);
                    }
                }
            }

            QuestMiniPanel.instance.UpdateQuestMiniPanel(
                QuestManager.instance.quests[QuestManager.current_quest_id]
            );
        }

        WearableAttribute attribute = item_info.GetAttributeOrNull<WearableAttribute>();
        //Check latest Slot which is Null
        for (int i = 0; i < inventory.Length; i++)
            if (inventory[i] == null)
            {
                if (remaining <= item_info.max_stackable_size)
                {
                    if (attribute != null)
                        inventory[i] = new ItemSave(
                            (int)item_info.id,
                            remaining,
                            (int)(
                                attribute.durability
                                * Skilltree.instance.GetBonusOfCategory(
                                    SkillData.TYPE_CATEGORY.TOOL_DURABILITY
                                )
                            ),
                            (int)item.state
                        );
                    else
                        inventory[i] = new ItemSave(
                            (int)item_info.id,
                            remaining,
                            -1,
                            (int)item.state
                        );

                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    UpdateSlot(i);
                    return;
                }
                else
                {
                    if (attribute != null)
                        inventory[i] = new ItemSave(
                            (int)item_info.id,
                            remaining,
                            (int)(
                                attribute.durability
                                * Skilltree.instance.GetBonusOfCategory(
                                    SkillData.TYPE_CATEGORY.TOOL_DURABILITY
                                )
                            ),
                            (int)item.state
                        );
                    else
                        inventory[i] = new ItemSave(
                            (int)item_info.id,
                            item_info.max_stackable_size,
                            (int)item.state
                        );
                    QuestMiniPanel.instance.UpdateQuestMiniPanel(
                        QuestManager.instance.quests[QuestManager.current_quest_id]
                    );
                    remaining -= item_info.max_stackable_size;
                }
            }
    }

    public Item GetLastItemFromInventoryOrNull(ItemSave[] inventory)
    {
        foreach (ItemSave item_save in inventory)
        {
            if (item_save == null)
                continue;
            return new Item(ITEM_TYPES[(ITEM_ID)item_save.item_id], 1);
        }
        return null;
    }

    public void ClearInventory()
    {
        //Check if Item already exists
        for (int i = 0; i < inventory_items.Length; i++)
        {
            if (inventory_items[i] == null)
                continue;

            // 4 - 6 = -2
            inventory_items[i] = null;
            UpdateSlot(i);
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

    public bool HasItemInInventory(ItemSave[] array, Item item)
    {
        //Check if Item already exists
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            if (item == null)
            {
                Debug.Print("ITEM IS NULL");
                return false;
            }
            if (array[i].item_id == (int)item.info.id)
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
        EquipmentPanel.UpdateSlots();
    }

    public override void UpdateSlot(int index, SlotItemUI pref_ref = null)
    {
        if (inventory_items[index] == null)
        {
            ClearSlot(index);
            return;
        }
        Debug.Print("State in Slot: " + inventory_items[index].state);
        GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0")
            .UpdateItem(
                new Item(
                    GetItemInfo(index),
                    inventory_items[index].amount,
                    (Item.STATE)inventory_items[index].state
                ),
                GetDurability(index)
            );

        if (pref_ref != null)
            GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0")
                .GetSlotItemUI()
                .SelfModulate = pref_ref.SelfModulate;

        QuestMiniPanel.instance.UpdateQuestMiniPanel(
            QuestManager.instance.quests[QuestManager.current_quest_id]
        );
    }

    public override void ClearSlot(int index)
    {
        Debug.Print("Inventory: Slot with index " + index + " cleared");
        GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0").ClearSlotItem();
    }

    //TODO: GetItemInfo ziemlich oft vertreten! Refactorn!
    public override ItemInfo GetItemInfo(int index)
    {
        return ITEM_TYPES[(ITEM_ID)inventory_items[index].item_id];
    }

    public int GetDurability(int index)
    {
        WearableAttribute attribute = GetItemInfo(index).GetAttributeOrNull<WearableAttribute>();
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
