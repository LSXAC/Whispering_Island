using Godot;

public partial class placeable_building : Building_Node
{
    [Export]
    public string building_name;

    [Export]
    public Texture2D building_texture;
    public bool colliding_Wall = false;
    public Building_Collider_Manager building_collider_manager;

    [Export]
    public Sprite2D sprite;

    [Export]
    public Node2D building_content;

    [Export]
    public float count = 0;

    [Export]
    public float export_count = 0;

    [Export]
    public bool is_crafter = false;

    [Export]
    public Giver giver;

    public override void OnMouseClick()
    {
        if (Global.GetDistanceToPlayer(this.GlobalPosition) < 60f)
            GD.Print("YESSSSSS");
        else
            GD.Print("No! No! No!");
    }

    public override void _Ready()
    {
        building_collider_manager = GetNode<Node2D>("BuildingAreas") as Building_Collider_Manager;
    }

    public void OnSpawnTimeout()
    {
        count++;
    }
}
