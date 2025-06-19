using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class itemRecipeUI : Control
{
    [Export]
    public Item_Row_Manager item_row_manager;
    public CraftingMenu craftingMenu;

    [Export]
    public Button craft_button;

    [Export]
    public bool can_craft = false;

    [Export]
    public Array<Item> req_items;

    [Export]
    public int button_id = -1;

    [Export]
    public Item output_item;

    [Export]
    public h_box_item output_item_hbox;

    public void InitResourceItems(Array<Item> items, Item output_item)
    {
        this.output_item = output_item;
        req_items = items;
        can_craft = false;

        if (item_row_manager.CanCreate(items))
            can_craft = true;

        if (
            !Inventory.instance.CanReceiveItem(
                output_item.item_info,
                Inventory.instance.inventory_items,
                output_item.amount
            )
        )
        {
            //player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_INVENTORY_FULL"));
            can_craft = false;
        }
    }

    public void CraftItem()
    {
        foreach (Item items in req_items)
        {
            Inventory.instance.RemoveItem(
                items.item_info,
                items.amount,
                Inventory.instance.inventory_items
            );
            Debug.Print(items.item_info.item_name);
        }

        Inventory.instance.AddItem(
            output_item.item_info,
            output_item.amount,
            Inventory.instance.inventory_items
        );
        craftingMenu.ReloadUIRecipes();
    }
}
