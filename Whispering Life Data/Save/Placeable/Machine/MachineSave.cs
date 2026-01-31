using Godot;

public partial class MachineSave : PlaceableSave
{
    public MachineSave() { }

    public MachineSave(Database.BUILDING_ID id, Vector2 pos, Vector2 scale, bool me)
    {
        this.building_id = id;
        this.pos = pos;
        this.scale = scale;
        this.machine_enabled = me;
    }

    [Export]
    public ItemSave[] furnace_slots = new ItemSave[3] { null, null, null };

    [Export]
    public int fuel_left = 0;

    [Export]
    public Vector2 scale;

    [Export]
    public bool machine_enabled = true;

    [Export]
    public int current_recipe = 0;

    [Export]
    public int count = 0;

    [Export]
    public ItemSave[] chest_items = new ItemSave[20];

    [Export]
    public ItemSave[] second_chest_items = new ItemSave[20];
}
