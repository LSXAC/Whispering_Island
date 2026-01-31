using System;
using Godot;

public partial class ProductionSave : PlaceableSave
{
    public ProductionSave() { }

    public ProductionSave(Database.BUILDING_ID id, Vector2 pos, Vector2 scale, bool me)
    {
        this.building_id = id;
        this.pos = pos;
        this.scale = scale;
        this.machine_enabled = me;
    }

    [Export]
    public int count = 0;

    [Export]
    public bool machine_enabled = true;

    [Export]
    public Vector2 scale;
}
