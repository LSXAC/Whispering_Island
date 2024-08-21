using System;
using Godot;

public partial class IslandSave : Resource
{
    [Export]
    public Island_Properties.DIRECTION dir = Island_Properties.DIRECTION.NONE;

    [Export]
    public int start_unique_island_id = -1;

    [Export]
    public int end_unique_island_id = -1;

    public IslandSave() { }

    public IslandSave(Island_Properties.DIRECTION dir, int start_unique_id, int end_unique_id)
    {
        this.dir = dir;
        this.start_unique_island_id = start_unique_id;
        this.end_unique_island_id = end_unique_id;
    }
}
