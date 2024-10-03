using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class itemRecipeUI : Control
{
    [Export]
    public Control parent;
    public CraftingMenu craftingMenu;

    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>("res://h_box_item.tscn");

    [Export]
    public Button craft_button;

    [Export]
    public h_box_item outputUI;

    [Export]
    public bool can_craft = false;

    [Export]
    public Array<Item> req_items;

    [Export]
    public int button_id = -1;

    [Export]
    public Item output_item;

    public void InitResourceItems(Array<Item> items, Item output_item)
    {
        this.output_item = output_item;
        req_items = items;
        int x = 0;
        can_craft = false;
        foreach (Item item in items)
        {
            h_box_item hbc_c = (h_box_item)h_box_item.Instantiate();
            hbc_c.InitItemUI("", item.amount, item.item_info.texture);

            hbc_c.ChangeColor(global::h_box_item.colorType.red);
            Item i_list = Inventory.INSTANCE.GetItemFromList(
                Inventory.INSTANCE.GetListOfItemsInInventory(),
                item
            );
            if (i_list != null)
            {
                if (i_list.amount >= item.amount)
                {
                    hbc_c.ChangeColor(global::h_box_item.colorType.white);
                    x++;
                }
            }

            parent.AddChild(hbc_c);
            parent = hbc_c;
        }
        if (x == req_items.Count)
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
