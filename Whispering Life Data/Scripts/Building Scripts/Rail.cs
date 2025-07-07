using System;
using System.Diagnostics;
using Godot;

public partial class Rail : TransportBase
{
    public void OnDetectorRailDetected(Area2D area)
    {
        if (ignore_self_detector)
            return;

        Debug.Print("Rail: " + area.Name);
        if (area is PathConnectArea)
        {
            if (area.GetParent() is Rail)
            {
                if (area.GetParent<TransportBase>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Rail>().receive_item(item);
                }
            }
        }
    }

    public void OnItemHolderItemHeld()
    {
        detector.Detect();
    }

    public override Resource Save()
    {
        bool has_minecart = false;
        Vector2 minecart_positon = Vector2.Zero;
        ItemSave[] chest_items = new ItemSave[20];
        if (item_holder.GetChildCount() > 0)
        {
            has_minecart = true;
            minecart_positon = item_holder.GetMinecart().Position;
            chest_items = item_holder.GetMinecart().chestBase.chest_items;
        }

        TransportBaseSave tbs = (TransportBaseSave)base.Save();
        RailSave rail_save = new RailSave(
            tbs,
            has_minecart,
            minecart_positon,
            chest_items,
            current_rotation
        );
        return rail_save;
    }

    public override void Load(Resource save)
    {
        if (save is RailSave rail_save)
        {
            base.Load(rail_save);
            if (rail_save.has_minecart)
            {
                Minecart cart =
                    Database
                        .GetBuildingMenuListChildObjectInfo(Database.BUILDING_ID.MINECART)
                        .scene.Instantiate() as Minecart;
                cart.chestBase.chest_items = rail_save.chest_items;
                item_holder.AddChild(cart);
                cart.Position = rail_save.minecart_position;
            }
        }
        else
            Logger.PrintWrongSaveType();
    }
}
