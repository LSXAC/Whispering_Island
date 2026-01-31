using Godot;

public partial class PlaceableSave : Resource
{
    public PlaceableSave() { }

    public PlaceableSave(Database.BUILDING_ID id, Vector2 pos)
    {
        this.building_id = id;
        this.pos = pos;
    }

    [Export]
    public Database.BUILDING_ID building_id;

    [Export]
    public Vector2 pos;
}
