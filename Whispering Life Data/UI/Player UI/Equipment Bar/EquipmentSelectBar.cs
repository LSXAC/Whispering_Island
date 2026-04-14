using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class EquipmentSelectBar : Container
{
    [Export]
    public Color normal_color,
        selected_color;

    [Export]
    public Array<Slot> select_slots = new Array<Slot>();

    public SlotItemUI current_selected_slot_item_ui = null;

    public static int current_selected_slot = 0;
    private double last_farmland_modification_time = -0.5;
    private const double FARMLAND_MODIFICATION_COOLDOWN = 0.3;

<<<<<<< HEAD
=======
    // Tool Marker für Visualisierung des angesteuerten Tiles
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
    private Sprite2D tool_marker;

    [Export]
    public Texture2D tool_marker_texture;

    public override void _Ready()
    {
<<<<<<< HEAD
=======
        // Tool Marker initialisieren
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        if (tool_marker == null)
        {
            tool_marker = new Sprite2D();
            tool_marker.Texture = tool_marker_texture;
<<<<<<< HEAD
            tool_marker.Modulate = new Color(1, 1, 1, 0.7f);
            GetTree().Root.AddChild(tool_marker);
            tool_marker.ZIndex = 10;
            tool_marker.Visible = false;
=======
            tool_marker.Modulate = new Color(1, 1, 1, 0.7f); // Semi-transparent
            GetTree().Root.AddChild(tool_marker); // In der Welt hinzufügen, nicht in der UI
            tool_marker.ZIndex = 10; // Über den Tiles
            tool_marker.Visible = false; // Zunächst unsichtbar
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        }
        SelectSelectSlot(0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NUM1"))
            SelectSelectSlot(0);

        if (Input.IsActionJustPressed("NUM2"))
            SelectSelectSlot(1);

        if (Input.IsActionJustPressed("NUM3"))
            SelectSelectSlot(2);

        if (Input.IsActionJustPressed("NUM4"))
            SelectSelectSlot(3);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsToolWithMenuActive())
            return;

        if (CutsceneManager.In_Cutscene)
            return;

        if (GameMenu.IsWindowActiv())
            return;

        UpdateToolMarker();

        if (Input.IsMouseButtonPressed(MouseButton.Right))
            HandleFarmlandTileModification();

        if (Input.IsMouseButtonPressed(MouseButton.Left))
            HandleFarmlandTileSet();
    }

    public SlotItemUI GetSelectedSlotItemUI()
    {
        return current_selected_slot_item_ui;
    }

    public void ClearSelectSlot(int index)
    {
        select_slots[index].ClearSlotItem();
        if (current_selected_slot == index)
            current_selected_slot_item_ui = null;
    }

    public void SetItemInSelectSlot(int index, SlotItemUI slot_item_ui)
    {
        select_slots[index]
            .SetItem(
                InventoryTab.clicked_slot_item_ui.item,
                InventoryTab.clicked_slot_item_ui.current_durability
            );
        if (current_selected_slot == index)
            SelectSelectSlot(index);
    }

    public MineableObject.MINING_LEVEL GetSelectedTypeLevel()
    {
        if (GetSelectedSlotItemUI() != null)
        {
            ToolAttribute attribute = GetSelectedSlotItemUI()
                .item.info.GetAttributeOrNull<ToolAttribute>();
            if (attribute != null)
                return attribute.mining_level;
        }
        return MineableObject.MINING_LEVEL.HAND;
    }

    public bool HasSameUseType(PlayerStats.TOOLTYPE tool_type)
    {
        if (GetSelectedTypeLevel() == MineableObject.MINING_LEVEL.HAND)
            return true;

        if (GetSelectedSlotItemUI() != null)
        {
            ToolAttribute attribute = GetSelectedSlotItemUI()
                .item.info.GetAttributeOrNull<ToolAttribute>();

            if (attribute != null)
                if (attribute.tool_type == tool_type)
                    return true;
        }
        return false;
    }

    public void SelectSelectSlot(int index)
    {
        tool_marker.Visible = false;
        select_slots[current_selected_slot].GetParent().GetParent().GetParent<ColorRect>().Color =
            normal_color;
        select_slots[index].GetParent().GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot_item_ui = select_slots[index].GetSlotItemUI();
        current_selected_slot = index;

        if (EquipmentPanel.instance != null)
            EquipmentPanel.instance.CalculateStatsFromEquipment();
    }

    private void ModifyFarmlandTile(
        TileMapLayer farmland,
        Vector2I tile_pos,
        ToolAttribute tool_attr
    )
    {
        var current_source_id = farmland.GetCellSourceId(tile_pos);
        var current_atlas = farmland.GetCellAtlasCoords(tile_pos);

        Debug.Print(
            $"[DEBUG ModifyFarmlandTile] Source ID: {current_source_id}, Atlas: {current_atlas}"
        );

        if (current_source_id >= 0)
        {
<<<<<<< HEAD
            farmland.SetCellsTerrainConnect(new Array<Vector2I> { tile_pos }, 0, -1);
=======
            farmland.SetCellsTerrainConnect(
                new Array<Vector2I> { tile_pos },
                0, // terrain_set (normalerweise 0)
                -1
            );
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
            Debug.Print($"[DEBUG ModifyFarmlandTile] Tile erased at position {tile_pos}");
        }
        else
        {
            Debug.Print($"[DEBUG ModifyFarmlandTile] No tile at position {tile_pos}");
        }
    }

    private bool IsToolWithMenuActive()
    {
        if (current_selected_slot_item_ui == null)
            return false;

        ToolAttribute tool_attr =
            current_selected_slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();

        if (tool_attr == null)
            return false;

        bool is_active = tool_attr.has_menu;
        return is_active;
    }

    private void UpdateToolMarker()
    {
        if (tool_marker == null || tool_marker_texture == null)
            return;

<<<<<<< HEAD
=======
        // Get mouse position in world coordinates
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        Camera2D camera = GetViewport().GetCamera2D();
        Vector2 mouse_world_pos =
            camera != null ? camera.GetGlobalMousePosition() : GetGlobalMousePosition();

        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (nearest_island == null || nearest_island.farmland_tilemap == null)
        {
            tool_marker.Visible = false;
            return;
        }

<<<<<<< HEAD
=======
        // Convert world position to tilemap local coordinates, then to tile coordinates
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        Vector2 mouse_tilemap_local_pos = nearest_island.farmland_tilemap.ToLocal(mouse_world_pos);
        Vector2I tile_under_mouse = nearest_island.farmland_tilemap.LocalToMap(
            mouse_tilemap_local_pos
        );

<<<<<<< HEAD
        Vector2 tile_center_local = nearest_island.farmland_tilemap.MapToLocal(tile_under_mouse);
        Vector2 tile_center_global = nearest_island.farmland_tilemap.ToGlobal(tile_center_local);

=======
        // Get the center position of the tile in global coordinates
        Vector2 tile_center_local = nearest_island.farmland_tilemap.MapToLocal(tile_under_mouse);
        Vector2 tile_center_global = nearest_island.farmland_tilemap.ToGlobal(tile_center_local);

        // Update marker position and visibility
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        tool_marker.GlobalPosition = tile_center_global;
        tool_marker.Visible = true;
    }

    private void HandleFarmlandTileModification()
    {
        double current_time = Time.GetTicksMsec() / 1000.0;
        if (current_time - last_farmland_modification_time < FARMLAND_MODIFICATION_COOLDOWN)
            return;

        Camera2D camera = GetViewport().GetCamera2D();
        Vector2 mouse_world_pos =
            camera != null ? camera.GetGlobalMousePosition() : GetGlobalMousePosition();

        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (nearest_island == null)
        {
            Debug.Print("[DEBUG] No island found under mouse");
            return;
        }

        if (nearest_island.farmland_tilemap == null)
        {
            Debug.Print("[DEBUG] Island has no farmland tilemap");
            return;
        }

<<<<<<< HEAD
=======
        // Convert world position to tilemap local coordinates, then to tile coordinates
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        Vector2 mouse_tilemap_local_pos = nearest_island.farmland_tilemap.ToLocal(mouse_world_pos);
        Vector2I tile_under_mouse = nearest_island.farmland_tilemap.LocalToMap(
            mouse_tilemap_local_pos
        );

<<<<<<< HEAD
=======
        // Check if there's actually a tile at this position
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        if (nearest_island.farmland_tilemap.GetCellSourceId(tile_under_mouse) < 0)
        {
            Debug.Print($"[DEBUG] No farmland tile at position {tile_under_mouse}");
            return;
        }

        ToolAttribute tool_attr =
            current_selected_slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();
        ModifyFarmlandTile(nearest_island.farmland_tilemap, tile_under_mouse, tool_attr);

        last_farmland_modification_time = Time.GetTicksMsec() / 1000.0;
    }

    private void HandleFarmlandTileSet()
    {
<<<<<<< HEAD
=======
        // Same cooldown mechanism
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        double current_time = Time.GetTicksMsec() / 1000.0;
        if (current_time - last_farmland_modification_time < FARMLAND_MODIFICATION_COOLDOWN)
            return;

        Camera2D camera = GetViewport().GetCamera2D();
        Vector2 mouse_world_pos =
            camera != null ? camera.GetGlobalMousePosition() : GetGlobalMousePosition();

        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (nearest_island == null)
        {
            Debug.Print("[DEBUG] No island found under mouse");
            return;
        }

        if (nearest_island.farmland_tilemap == null)
        {
            Debug.Print("[DEBUG] Island has no farmland tilemap");
            return;
        }

        ToolAttribute tool_attr =
            current_selected_slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();

        if (tool_attr == null || tool_attr.auto_tile_id < 0)
        {
            Debug.Print("[DEBUG] Tool has no valid farmland tile source ID");
            return;
        }

        Vector2 mouse_tilemap_local_pos = nearest_island.farmland_tilemap.ToLocal(mouse_world_pos);
        Vector2I tile_under_mouse = nearest_island.farmland_tilemap.LocalToMap(
            mouse_tilemap_local_pos
        );

<<<<<<< HEAD
        nearest_island.farmland_tilemap.SetCellsTerrainConnect(
            new Array<Vector2I> { tile_under_mouse },
            0,
            tool_attr.auto_tile_id
        );

=======
        Debug.Print(
            $"[DEBUG] Setting tile at {tile_under_mouse} with terrain ID {tool_attr.auto_tile_id}"
        );

        // Nutze SetCellsTerrainConnect für Terrain-basiertes Autotiling
        nearest_island.farmland_tilemap.SetCellsTerrainConnect(
            new Array<Vector2I> { tile_under_mouse },
            0, // terrain_set (normalerweise 0)
            tool_attr.auto_tile_id
        );

        // Debug: überprüfe, ob das Tile wirklich gesetzt wurde
>>>>>>> 8a3010b4e06625c2d66f8f5fde609f3972b4757e
        var set_source_id = nearest_island.farmland_tilemap.GetCellSourceId(tile_under_mouse);
        var set_atlas = nearest_island.farmland_tilemap.GetCellAtlasCoords(tile_under_mouse);
        Debug.Print(
            $"[DEBUG] After SetCellsTerrainConnect - Source ID: {set_source_id}, Atlas: {set_atlas}"
        );

        last_farmland_modification_time = Time.GetTicksMsec() / 1000.0;
    }
}
