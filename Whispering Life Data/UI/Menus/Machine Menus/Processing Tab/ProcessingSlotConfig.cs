using Godot;

public struct ProcessingSlotConfig
{
    public string slot_name;

    public SlotPurpose purpose;

    public Slot ui_reference;

    public string description;

    public ProcessingSlotConfig(
        string slot_name,
        SlotPurpose purpose,
        Slot ui_reference,
        string description = ""
    )
    {
        this.slot_name = slot_name;
        this.purpose = purpose;
        this.ui_reference = ui_reference;
        this.description = description;
    }
}

public enum SlotPurpose
{
    INPUT,
    OUTPUT,
    FUEL,
    AUXILIARY
}
