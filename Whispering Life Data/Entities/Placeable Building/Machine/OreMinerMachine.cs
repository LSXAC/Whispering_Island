using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class OreMinerMachine : ProductionMachine
{
    private System.Collections.Generic.Dictionary<Vector2I, ItemInfo> ore_atlas_to_item_mapping =
        new System.Collections.Generic.Dictionary<Vector2I, ItemInfo>();

    [Export]
    public string ore_tilemap_name = "OreArea";

    private TileMapLayer ore_tilemap;
    private bool ore_item_initialized = false;

    public override void _Ready()
    {
        base._Ready();
    }

    public void InitOreMiner()
    {
        InitializeOreTilemap();
        InitializeOreMapping();

        if (output_item_resource == null)
        {
            output_item_resource = GetItemFromOreTilemap();
            ore_item_initialized = true;
            if (output_item_resource == null)
                Debug.Print("[OreMinerMachine] Failed to auto-detect ore item");
        }
    }

    private void InitializeOreMapping()
    {
        try
        {
            if (Inventory.ITEM_TYPES.ContainsKey(Inventory.ITEM_ID.IRON_ORE))
                AddOreMapping(new Vector2I(1, 0), Inventory.ITEM_TYPES[Inventory.ITEM_ID.IRON_ORE]);

            if (Inventory.ITEM_TYPES.ContainsKey(Inventory.ITEM_ID.COAL))
                AddOreMapping(new Vector2I(2, 0), Inventory.ITEM_TYPES[Inventory.ITEM_ID.COAL]);

            if (Inventory.ITEM_TYPES.ContainsKey(Inventory.ITEM_ID.COPPER_ORE))
                AddOreMapping(
                    new Vector2I(0, 1),
                    Inventory.ITEM_TYPES[Inventory.ITEM_ID.COPPER_ORE]
                );
        }
        catch (Exception ex)
        {
            Debug.Print($"[OreMinerMachine] Error initializing ore mapping: {ex.Message}");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void InitializeOreTilemap()
    {
        if (IslandManager.instance == null)
            return;

        Island current_island = IslandManager.instance.GetNearestIsland(GlobalPosition);
        if (current_island == null)
            return;

        ore_tilemap = FindOreTilemapOnIsland(current_island);
        if (ore_tilemap == null)
            Debug.Print($"[OreMinerMachine] Tilemap '{ore_tilemap_name}' not found on island");
    }

    private TileMapLayer FindOreTilemapOnIsland(Island island)
    {
        if (island == null)
            return null;

        // Search for tilemap with the specified name in direct children
        foreach (Node child in island.GetChildren())
        {
            if (child is TileMapLayer tilemapLayer && child.Name == ore_tilemap_name)
                return tilemapLayer;
        }

        // If not found in direct children, search recursively
        foreach (Node child in island.GetChildren())
        {
            TileMapLayer found = SearchTilemapRecursive(child, ore_tilemap_name);
            if (found != null)
                return found;
        }

        return null;
    }

    private TileMapLayer SearchTilemapRecursive(Node node, string name)
    {
        if (node is TileMapLayer tilemapLayer && node.Name == name)
            return tilemapLayer;

        foreach (Node child in node.GetChildren())
        {
            TileMapLayer found = SearchTilemapRecursive(child, name);
            if (found != null)
                return found;
        }

        return null;
    }

    private ItemInfo GetItemFromOreTilemap()
    {
        if (ore_tilemap == null)
            InitializeOreTilemap();

        if (ore_tilemap == null)
            return null;

        Vector2 local_pos = ore_tilemap.ToLocal(GlobalPosition);
        Vector2I tile_pos = ore_tilemap.LocalToMap(local_pos);
        Vector2I atlas_coords = ore_tilemap.GetCellAtlasCoords(tile_pos);

        // If there's no valid tile, try neighboring tiles
        if (atlas_coords == new Vector2I(-1, -1))
        {
            ItemInfo neighbor_item = TryNeighboringTiles(tile_pos);
            if (neighbor_item != null)
                return neighbor_item;
            return null;
        }

        // Look up the item based on atlas coordinates
        if (ore_atlas_to_item_mapping.ContainsKey(atlas_coords))
            return ore_atlas_to_item_mapping[atlas_coords];

        return null;
    }

    private ItemInfo TryNeighboringTiles(Vector2I center_pos)
    {
        Vector2I[] neighbors = new Vector2I[]
        {
            center_pos + new Vector2I(-1, -1),
            center_pos + new Vector2I(0, -1),
            center_pos + new Vector2I(1, -1),
            center_pos + new Vector2I(-1, 0),
            center_pos + new Vector2I(1, 0),
            center_pos + new Vector2I(-1, 1),
            center_pos + new Vector2I(0, 1),
            center_pos + new Vector2I(1, 1)
        };

        foreach (Vector2I neighbor in neighbors)
        {
            Vector2I atlas_coords = ore_tilemap.GetCellAtlasCoords(neighbor);
            if (
                atlas_coords != new Vector2I(-1, -1)
                && ore_atlas_to_item_mapping.ContainsKey(atlas_coords)
            )
                return ore_atlas_to_item_mapping[atlas_coords];
        }

        return null;
    }

    public ItemInfo GetCurrentOreItem()
    {
        return GetItemFromOreTilemap();
    }

    public void AddOreMapping(Vector2I atlas_coords, ItemInfo item)
    {
        ore_atlas_to_item_mapping[atlas_coords] = item;
    }

    public void ClearOreMapping()
    {
        ore_atlas_to_item_mapping.Clear();
    }
}
