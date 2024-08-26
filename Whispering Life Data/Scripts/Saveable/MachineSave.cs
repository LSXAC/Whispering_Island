using Godot;

public partial class MachineSave : Resource
{
    public MachineSave() { }

    public MachineSave(MachineBase.MachineType type, Vector2 pos, int im_c, int ex_c)
    {
        this.type = type;
        this.position = pos;
        this.import_count = im_c;
        this.export_count = ex_c;
    }

    [Export]
    public MachineBase.MachineType type = MachineBase.MachineType.WOODFARM;

    [Export]
    public Vector2 position = Vector2.Zero;

    [Export]
    public int import_count = 0;

    [Export]
    public int export_count = 0;
}
