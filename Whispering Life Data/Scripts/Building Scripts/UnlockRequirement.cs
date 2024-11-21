using Godot;

[GlobalClass]
public partial class UnlockRequirement : Resource
{
    [Export]
    public Database.UPGRADE_LEVEL required_level = Database.UPGRADE_LEVEL.Level1;

    [Export]
    public InventoryBase.ITEM_ID item_id;
}
