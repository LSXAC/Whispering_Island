using System;
using Godot;

public partial class WorldMapData : TileMapLayer
{
    [Export]
    public WorldMap.WORLDTILE tile;

    [Export]
    public WorldMap.LAYER layer;

    [Export]
    public Island island;

    public override void _Ready()
    {
        AddTileMapWorldData();
    }

    public void AddTileMapWorldData()
    {
        WorldMap.data.Add(
            new TileMapWorldData(
                new Vector2(island.matrix_x, island.matrix_y),
                this,
                layer,
                tile,
                TileMapWorldData.STATE.ADD
            )
        );
    }

    /*public void RemoveTileMapFromWorldMap()
    {
        if (WorldMap.instance == null)
            return;

        if (tile == WorldMap.WORLDTILE.GREEN_NORMAL || tile == WorldMap.WORLDTILE.GREEN_TOP)
            WorldMap.instance.ground_layer.SetCellv(this.Position, -1);
        else if (tile == WorldMap.WORLDTILE.MYSTIC_NORMAL || tile == WorldMap.WORLDTILE.MYSTIC_TOP)
            WorldMap.instance.top_layer.SetCellv(this.Position, -1);
        else if (tile == WorldMap.WORLDTILE.MINING_NORMAL || tile == WorldMap.WORLDTILE.MINING_TOP)
            WorldMap.instance.bridge_layer.SetCellv(this.Position, -1);
    }*/
}
