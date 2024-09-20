using Godot;

public partial class MachineSave : Resource
{
    public MachineSave() { }

    public MachineSave(MachineBase.MachineType type, Vector2 pos, Vector2 scale, bool me)
    {
        this.type = type;
        this.position = pos;
        this.scale = scale;
        this.machine_enabled = me;
    }

    [Export]
    public MachineBase.MachineType type = MachineBase.MachineType.WOODFARM;

    [Export]
    public Vector2 position = Vector2.Zero;

    [Export]
    public int import_count = 0;

    [Export]
    public int import_item_type = -1;

    [Export]
    public int export_item_type = -1;

    [Export]
    public int export_count = 0;

    [Export]
    public Vector2 scale;

    [Export]
    public bool machine_enabled = true;

    [Export]
    public int current_recipe = 0;

    [Export]
    public ItemSave[] chest_items = new ItemSave[20];
}
