using System;
using Godot;
using Godot.Collections;

public partial class IslandSave : Resource
{
    [Export]
    public Island_Properties.DIRECTION dir = Island_Properties.DIRECTION.NONE;

    [Export]
    public int matrix_island_id = -1;

    [Export]
    public int island_id = -1;

    public IslandSave() { }

    public IslandSave(Island_Properties.DIRECTION dir, int matrix_island_id, int island_id)
    {
        this.dir = dir;
        this.matrix_island_id = matrix_island_id;
        this.island_id = island_id;
    }
}
