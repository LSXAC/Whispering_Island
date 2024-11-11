using Godot;

public partial class placeable_building : Building_Node
{
    public bool colliding_Wall = false;

    [Export]
    public Building_Collider_Manager building_collider_manager;

    public override void OnMouseClick()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
            QueueFree();

        if (Global.GetDistanceToPlayer(this.GlobalPosition) < 60f) { }
        else
            GD.Print("No! No! No!");
    }
}
