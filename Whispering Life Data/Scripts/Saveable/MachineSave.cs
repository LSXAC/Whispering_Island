using Godot;

public partial class MachineSave : Resource
{
    public MachineSave() { }

    public MachineSave(
        MachineBase.MachineType type,
        Vector2 pos,
        Vector2 scale,
        int im_c,
        int ex_c,
        bool me,
        ItemInfo import_item_info,
        ItemInfo export_item_info
    )
    {
        this.type = type;
        this.position = pos;
        this.import_count = im_c;
        this.export_count = ex_c;
        this.scale = scale;
        this.machine_enabled = me;
        if (import_item_info == null)
            this.import_item_type = -1;
        else
            this.import_item_type = import_item_info.unique_item_id;

        if (export_item_info == null)
            this.export_item_type = -1;
        else
            this.export_item_type = export_item_info.unique_item_id;
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
}
