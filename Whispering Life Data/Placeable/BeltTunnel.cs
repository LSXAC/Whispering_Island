using System;
using System.Diagnostics;
using Godot;

public partial class BeltTunnel : Belt
{
    [Export]
    public bool is_tunnel_connected = false;

    [Export]
    public ItemHolder connected_itemholder;

    [Export]
    public Timer checkAreaTimer;

    [Export]
    public bool from_Belt = false;

    public static int length = 16;
    public bool break_search = false;
    Area2D checkArea;

    public override void _Ready()
    {
        checkArea = GetNode<Area2D>("TunnelArea");
    }

    public new bool can_receive_item()
    {
        if (connected_itemholder == null)
            return false;

        return connected_itemholder.GetChildCount() == 0;
    }

    public void CheckTimerTimeout()
    {
        if (!is_tunnel_connected)
            return;

        if (connected_itemholder == null)
            return;

        if (!from_Belt)
            return;

        if (connected_itemholder.GetParent<BeltTunnel>().item_holder.GetChildCount() == 0)
        {
            Debug.Print("offload from Tunnel A");
            var item = item_holder.offload_item();
            connected_itemholder.GetParent<Belt>().receive_item(item);
        }
    }

    public async void CheckIfTunnelInDir()
    {
        checkAreaTimer.Start();
        for (int i = 0; i < 10; i++)
        {
            //    <<<<<<y>>>>>>     <<<<<<x>>>>>>
            if (to_direction == BeltDirection.Top)
                checkArea.Position += new Vector2(0, -16);
            if (to_direction == BeltDirection.Down)
                checkArea.Position += new Vector2(0, 16);
            if (to_direction == BeltDirection.Left)
                checkArea.Position += new Vector2(-16, 0);
            if (to_direction == BeltDirection.Right)
                checkArea.Position += new Vector2(16, 0);

            await ToSignal(checkAreaTimer, "timeout");
            if (break_search)
            {
                break_search = false;
                Debug.Print("Breaked Loop");
                ResetCheckArea();
                break;
            }
        }
        ResetCheckArea();
        for (int i = 0; i < 10; i++)
        {
            //    <<<<<<y>>>>>>     <<<<<<x>>>>>>
            if (from_direction == BeltDirection.Top)
                checkArea.Position += new Vector2(0, -16);
            if (from_direction == BeltDirection.Down)
                checkArea.Position += new Vector2(0, 16);
            if (from_direction == BeltDirection.Left)
                checkArea.Position += new Vector2(-16, 0);
            if (from_direction == BeltDirection.Right)
                checkArea.Position += new Vector2(16, 0);

            await ToSignal(checkAreaTimer, "timeout");
            if (break_search)
            {
                break_search = false;
                Debug.Print("Breaked Loop");
                ResetCheckArea();
                break;
            }
        }
        ResetCheckArea();
        checkAreaTimer.Stop();
    }

    public void ResetCheckArea()
    {
        checkArea.Position = Vector2.Zero;
    }
}
