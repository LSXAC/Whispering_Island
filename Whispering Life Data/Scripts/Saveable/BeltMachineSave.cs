using Godot;

public partial class BeltMachineSave : Resource
{
    public BeltMachineSave() { }

    [Export]
    public Database.BUILDING_ID id;

    public BeltMachineSave(BeltSave b1, BeltSave b2, BeltSave b3, BeltSave b4)
    {
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
        this.b4 = b4;
    }

    [Export]
    public BeltSave b1,
        b2,
        b3,
        b4;
}
