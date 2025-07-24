using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class WorldMap : CanvasLayer
{
    public static WorldMap instance = null;
    public static Array<TileMapWorldData> data = new Array<TileMapWorldData>();

    [Export]
    public TileMapLayer ground_layer,
        top_layer,
        bridge_layer;

    [Export]
    public ScrollContainer scroll_container;

    [Export]
    public TextureRect player_texture_rect;

    public enum LAYER
    {
        GROUND,
        TOP,
        BRIDGE
    }

    public enum WORLDTILE
    {
        GREEN_NORMAL,
        GREEN_TOP,
        MYSTIC_NORMAL,
        MYSTIC_TOP,
        MINING_NORMAL,
        MINING_TOP,
        FARMING_NORMAL,
        FARMING_TOP,
        DESSERT_NORMAL,
        DESSERT_TOP,
        BRIDGE
    }

    public override void _Ready()
    {
        instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GameManager.gameover || GameManager.In_Cutscene)
            return;

        if (Input.IsActionJustPressed("Map"))
        {
            UpdateMap();
            SetPlayerPositionToWorldMap();
            GameMenu.SetWindow(this);
        }

        if (Input.IsActionJustPressed("Escape"))
        {
            CloseMap();
        }
    }

    public void SetPlayerPositionToWorldMap()
    {
        // Spielerposition in TileMap-Koordinaten
        Island island = IslandManager.instance.GetNearestIsland(Player.instance.GlobalPosition);

        Vector2I offset = new Vector2I(
            (int)island.matrix_x * 16 * 8,
            (int)island.matrix_y * 16 * 8
        );
        Vector2I base_pos = new Vector2I(1928 / 2, 2232 / 2);
        Vector2I new_pos = offset + base_pos;

        // Setze Scroll-Position so, dass Spieler mittig angezeigt wird
        scroll_container.ScrollVertical = new_pos.Y;
        scroll_container.ScrollHorizontal = new_pos.X;
        player_texture_rect.Position = GlobalPositionToWorldMap(
            Player.instance.GlobalPosition + new Vector2(-16, -16)
        );
    }

    public Vector2 GlobalPositionToWorldMap(Vector2 position)
    {
        Vector2I player_base_pos = new Vector2I(1280, 1280); // Center position in tile units
        return player_base_pos + position / 4;
    }

    public Vector2I GetTile(WORLDTILE tile)
    {
        switch (tile)
        {
            case WORLDTILE.GREEN_NORMAL:
                return new Vector2I(0, 0);
            case WORLDTILE.GREEN_TOP:
                return new Vector2I(1, 0);
            case WORLDTILE.MYSTIC_NORMAL:
                return new Vector2I(0, 1);
            case WORLDTILE.MYSTIC_TOP:
                return new Vector2I(1, 1);
            case WORLDTILE.FARMING_NORMAL:
                return new Vector2I(0, 2);
            case WORLDTILE.FARMING_TOP:
                return new Vector2I(1, 2);
            case WORLDTILE.DESSERT_NORMAL:
                return new Vector2I(0, 3);
            case WORLDTILE.DESSERT_TOP:
                return new Vector2I(1, 3);
            case WORLDTILE.MINING_NORMAL:
                return new Vector2I(0, 4);
            case WORLDTILE.MINING_TOP:
                return new Vector2I(1, 4);
            case WORLDTILE.BRIDGE:
                return new Vector2I(2, 0);
        }
        return new Vector2I(0, 0); // Default case
    }

    private void UpdateMap()
    {
        foreach (TileMapWorldData tilemap_data in data)
        {
            if (tilemap_data.state == TileMapWorldData.STATE.ADD)
            {
                SetTileMapToWorldMap(
                    tilemap_data.island_offset,
                    tilemap_data.tilemap,
                    tilemap_data.layer,
                    tilemap_data.tile
                );
            }
            else if (tilemap_data.state == TileMapWorldData.STATE.REMOVE)
            {
                // Logic to remove the tile map from the world map
                // This could be implemented as needed
            }
        }
        data.Clear(); // Clear the data after updating the map
    }

    public void SetTileMapToWorldMap(
        Vector2 island_offset,
        TileMapLayer tilemap,
        LAYER layer,
        WORLDTILE tile
    )
    {
        foreach (Vector2I pos in tilemap.GetUsedCells())
            SetSingleTileToWorldMap(island_offset, pos, layer, tile);
    }

    public void SetSingleTileToWorldMap(
        Vector2 island_offset,
        Vector2I position,
        LAYER layer,
        WORLDTILE tile
    )
    {
        // Calculate offset in tile units (not pixels)
        Vector2I offset = new Vector2I((int)island_offset.X * 16, (int)island_offset.Y * 16);
        Vector2I base_pos = new Vector2I(1280 / 8, 1280 / 8); // Center position in tile units

        Vector2I new_pos = base_pos + position + offset;
        if (layer == LAYER.GROUND)
            ground_layer.SetCell(new_pos, 0, GetTile(tile));
        else if (layer == LAYER.TOP)
            top_layer.SetCell(new_pos, 0, GetTile(tile));
        else if (layer == LAYER.BRIDGE)
            bridge_layer.SetCell(new_pos, 0, GetTile(tile));
    }

    public void CloseMap()
    {
        GameMenu.CloseLastWindow();
    }
}

public partial class TileMapWorldData : Node2D
{
    public Vector2 island_offset;
    public TileMapLayer tilemap;
    public WorldMap.LAYER layer;
    public WorldMap.WORLDTILE tile;
    public STATE state;

    public enum STATE
    {
        ADD,
        REMOVE
    }

    public TileMapWorldData(
        Vector2 island_offset,
        TileMapLayer tilemap,
        WorldMap.LAYER layer,
        WorldMap.WORLDTILE tile,
        STATE state
    )
    {
        this.island_offset = island_offset;
        this.tilemap = tilemap;
        this.layer = layer;
        this.tile = tile;
        this.state = state;
    }
}
