using System;
using Godot;

public partial class BeltTransmitterSave : Resource
{
    [Export]
    public bool is_connected = false;

    [Export]
    public BeltSave beltsave1;

    [Export]
    public BeltSave beltsave2;

    public BeltTransmitterSave() { }

    public BeltTransmitterSave(BeltSave beltsave1, bool is_connected, BeltSave beltsave2)
    {
        this.beltsave1 = beltsave1;
        this.is_connected = is_connected;
        this.beltsave2 = beltsave2;
    }
}
