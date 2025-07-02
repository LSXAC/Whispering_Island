using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class itemRecipeUI : Control
{
    [Export]
    public ItemRowManager item_row_manager;
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

        if (item_row_manager.CheckEnoughResources(items))
            can_craft = true;

        if (
            !PlayerInventoryUI.instance.CanReceiveItem(
                new Item(output_item.info, output_item.amount),
                PlayerInventoryUI.instance.inventory_items
            )
        )
        {
            //player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_INVENTORY_FULL"));
            can_craft = false;
        }
    }

    public void CraftItem()
    {
        foreach (Item item in req_items)
        {
            PlayerInventoryUI.instance.RemoveItem(
                new Item(item.info, item.amount),
                PlayerInventoryUI.instance.inventory_items
            );
            Debug.Print(item.info.name);
        }

        PlayerInventoryUI.instance.AddItem(
            new Item(output_item.info, output_item.amount),
            PlayerInventoryUI.instance.inventory_items
        );
        craftingMenu.ReloadUIRecipes();
    }
}
