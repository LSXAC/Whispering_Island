using System;
using Godot;

public partial class BeltTunnel : Belt
{
    [Export]
    public bool is_tunnel_connected = false;

    [Export]
    public ItemHolder connected_itemholder;

    [Export]
    public Timer checkAreaTimer;

    public static int length = 10;
    public bool break_search = false;
    Area2D checkArea;

    public override void _Ready()
    {
        checkArea = GetNode<Area2D>("TunnelArea");
        checkAreaTimer.Start();
        CheckIfTunnelInDir();
    }

    public new bool can_receive_item()
    {
        if (connected_itemholder == null)
            return false;

        return connected_itemholder.GetChildCount() == 0;
    }

    public async void CheckIfTunnelInDir()
    {
        for (int i = 0; i < 3; i++)
        {
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
                ResetCheckArea();
                break;
            }
        }
    }

    public void ResetCheckArea()
    {
        checkArea.Position = Vector2.Zero;
    }
}
