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
            hbc_c.InitItemUI("", item.amount, item.info.texture);
            hbc_c.ChangeColor(global::h_box_item.colorType.red);

            Array<Item> i_list = PlayerInventoryUI.instance.GetItemFromList(
                PlayerInventoryUI.instance.GetListOfItemsInInventory(),
                item
            );
            int amount_of_item = 0;
            if (i_list != null)
                foreach (Item i in i_list)
                    amount_of_item += i.amount;

            if (i_list != null)
            {
                if (amount_of_item >= item.amount)
                {
                    if (item.amount > 0)
                    {
                        amount_of_each_item[item] = amount_of_item / item.amount;
                        hbc_c.ChangeColor(global::h_box_item.colorType.white);
                    }
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
                if (times > amount)
                    times = amount;
            }
            PlayerUI.instance.times_to_build_left_label.Text =
                "> " + times + "x " + TranslationServer.Translate("PLAYERUI_TIMES_LEFT_TO_BUILD");

            return true;
        }
        PlayerUI.instance.times_to_build_left_label.Text =
            "> 0x " + TranslationServer.Translate("PLAYERUI_TIMES_LEFT_TO_BUILD");
        ;
        return false;
    }
}
