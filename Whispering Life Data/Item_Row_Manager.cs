using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Item_Row_Manager : HBoxContainer
{
    private PackedScene h_box_item = ResourceLoader.Load<PackedScene>("res://h_box_item.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public bool CanCreate(Array<Item> items)
    {
        foreach (Control c in GetChildren())
            c.QueueFree();

        if (items == null)
            return false;

        int x = 0;
        Dictionary<Item, int> amount_of_each_item = new Dictionary<Item, int>();
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
                    amount_of_each_item[i_list] = i_list.amount / item.amount;
                    hbc_c.ChangeColor(global::h_box_item.colorType.white);
                    x++;
                }
            }
            AddChild(hbc_c);
        }
        if (x == items.Count)
        {
            var (Item, amount_first) = amount_of_each_item.First();
            int times = amount_first;

            foreach (var (item, amount) in amount_of_each_item)
            {
                // 4, 2, 3
                if (times > amount)
                    times = amount;
            }
            player_ui.INSTANCE.times_to_build_left_label.Text = "> " + times + "x Left ";
            return true;
        }
        player_ui.INSTANCE.times_to_build_left_label.Text = "> 0x Left ";
        return false;
    }
}
