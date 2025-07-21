using System;
using Godot;
using Godot.Collections;

public partial class RemovableObjectsManager : Node2D
{
    [Export]
    public Array<int> removed_objects = new Array<int>();

    [Export]
    public Node2D tilemap_parent,
        sign_parent;

    public void RemoveSigns(Array<int> removed_signs)
    {
        foreach (int rs_id in removed_signs)
        {
            foreach (RemoveSign r_sign in sign_parent.GetChildren())
            {
                if (r_sign.id != rs_id)
                    continue;
                r_sign.object_connected.QueueFree();
                r_sign.QueueFree();
            }
        }
    }
}
