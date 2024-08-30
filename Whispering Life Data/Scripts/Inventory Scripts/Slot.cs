using System;
using System.Diagnostics;
using Godot;

public partial class Slot : PanelContainer
{
    [Export]
    public ItemInfo.Type type;

    public override void _Ready() { }

    public void SetItem(InventoryItem ii, int amount)
    {
        //type in DropData needs to be clearafied
        //type = ii.item_info.item_type;
        ii.amount = amount;
        ii.UpdateAmountLabel();
        AddChild(ii);
        if (amount == 0)
            ClearItem();
    }

    public void UpdateItem(int amount)
    {
        GetItem().amount += amount;
        GetItem().UpdateAmountLabel();
        if (GetItem().amount == 0)
            ClearItem();
    }

    public void UpdateFurnaceItem(InventoryItem ii, int amount)
    {
        if (GetItem() == null)
            SetItem(ii, amount);
        GetItem().amount += amount;
        GetItem().UpdateAmountLabel();
        if (GetItem().amount <= 0)
            ClearItem();
    }

    public InventoryItem GetItem()
    {
        foreach (Node child in GetChildren())
            return (InventoryItem)child;
        return null;
    }

    public void ClearItem()
    {
        if (GetItem() == null)
            return;
        GetItem().QueueFree();
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.Obj == null)
            return false;

        InventoryItem ii = (InventoryItem)data;
        if (type == ItemInfo.Type.Resource)
        {
            if (GetChildCount() == 0)
                return true;
            else if (type == ii.GetParent<Slot>().type)
                return true;
        }
        else
        {
            return ii.item_info.item_type == type;
        }

        return false;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.Obj == null)
            return;

        InventoryItem ii = (InventoryItem)data;
        if (GetChildCount() > 0)
        {
            InventoryItem item = GetChild<InventoryItem>(0);
            if ((Node)data == item)
                return;

            if (item.item_info == ii.item_info)
            {
                item.amount += ii.amount;
                item.UpdateAmountLabel();
                ii.QueueFree();
                return;
            }
            item.Reparent(((Node)data).GetParent());
        }
        ((Node)data).Reparent(this);
    }
}
