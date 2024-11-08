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
    }

    public void CraftItem()
    {
        foreach (Item items in req_items)
        {
            Inventory.INSTANCE.AddItem(
                items.item_info,
                -items.amount,
                Inventory.INSTANCE.inventory_items
            );
            Debug.Print(items.item_info.item_name);
        }

        Inventory.INSTANCE.AddItem(
            output_item.item_info,
            output_item.amount,
            Inventory.INSTANCE.inventory_items
        );
        craftingMenu.ReloadUIRecipes();
    }
}
