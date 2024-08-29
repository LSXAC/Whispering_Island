using Godot;

public partial class placeable_building : Building_Node
{
    public bool colliding_Wall = false;
    public Building_Collider_Manager building_collider_manager;

    public override void OnMouseClick()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
            QueueFree();

        if (Global.GetDistanceToPlayer(this.GlobalPosition) < 60f) { }
        else
            GD.Print("No! No! No!");
    }

    public override void _Ready()
    {
        building_collider_manager = GetNode<Node2D>("BuildingAreas") as Building_Collider_Manager;
    }
}
