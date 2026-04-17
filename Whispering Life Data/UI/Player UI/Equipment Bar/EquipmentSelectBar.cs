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

    private Node2D tool_marker_container;
    private Sprite2D tool_marker;
    private BuildingColliderManager tool_marker_collider_manager;

    [Export]
    public Texture2D tool_marker_texture;

    [Export]
    private PackedScene building_collider_scene = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://b5u81hi3w1yvx")
    );

    private ToolAttribute current_tool_attr = null;

    public override void _Ready()
    {
        if (tool_marker == null)
        {
            tool_marker_container = new Node2D();
            GetTree().Root.AddChild(tool_marker_container);
            tool_marker_container.ZIndex = 10;

            tool_marker = new Sprite2D();
            tool_marker.Texture = tool_marker_texture;
            tool_marker.Modulate = new Color(1, 1, 1, 0.7f);
            tool_marker_container.AddChild(tool_marker);

            tool_marker_collider_manager = new BuildingColliderManager();
            tool_marker_container.AddChild(tool_marker_collider_manager);

            if (Logger.NodeIsNotNull(building_collider_scene))
            {
                BuildingCollider building_collider = (BuildingCollider)
                    building_collider_scene.Instantiate();
                tool_marker_collider_manager.AddChild(building_collider);
                building_collider.Position = new Vector2(0, 0);
            }

            tool_marker_container.Visible = false;
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
        if (!ShouldProcessToolInput())
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
        tool_marker_container.Visible = false;
        select_slots[current_selected_slot].GetParent().GetParent().GetParent<ColorRect>().Color =
            normal_color;
        select_slots[index].GetParent().GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot_item_ui = select_slots[index].GetSlotItemUI();
        current_selected_slot = index;

        current_tool_attr =
            current_selected_slot_item_ui != null
                ? current_selected_slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>()
                : null;

        if (EquipmentPanel.instance != null)
            EquipmentPanel.instance.CalculateStatsFromEquipment();
    }

    public void HideToolMarker()
    {
        if (tool_marker_container != null)
            tool_marker_container.Visible = false;
    }

    private void HandleFarmlandTileModification()
    {
        if (!IsModificationCooldownExpired())
            return;

        Vector2 mouse_world_pos = GetMouseWorldPosition();
        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (!IsValidIsland(nearest_island))
            return;

        Vector2I tile_under_mouse = GetTileUnderMouse(nearest_island, mouse_world_pos);

        if (!HasFarmlandTile(nearest_island, tile_under_mouse))
            return;

        if (!CanRemoveTile())
            return;

        ModifyFarmlandTile(nearest_island.farmland_tilemap, tile_under_mouse, current_tool_attr);
        ReduceDurability();
        UpdateModificationTime();
    }

    private void HandleFarmlandTileSet()
    {
        if (!IsModificationCooldownExpired())
            return;

        Vector2 mouse_world_pos = GetMouseWorldPosition();
        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (!IsValidIsland(nearest_island))
            return;

        if (!HasValidToolAttribute())
            return;

        if (!CanPlaceTile())
            return;

        Vector2I tile_under_mouse = GetTileUnderMouse(nearest_island, mouse_world_pos);
        PlaceFarmlandTile(nearest_island, tile_under_mouse);
        ReduceDurability();
        UpdateModificationTime();
    }

    private bool ShouldProcessToolInput()
    {
        if (!IsToolWithMenuActive())
            return false;

        if (CutsceneManager.In_Cutscene)
            return false;

        if (GameMenu.IsWindowActiv())
            return false;

        return true;
    }

    private bool IsToolWithMenuActive()
    {
        if (!HasValidSelectedSlot())
            return false;

        ToolAttribute tool_attr = GetCurrentToolAttribute();
        return tool_attr != null && tool_attr.has_menu;
    }

    private bool HasValidSelectedSlot()
    {
        return current_selected_slot_item_ui != null;
    }

    private ToolAttribute GetCurrentToolAttribute()
    {
        if (current_selected_slot_item_ui == null)
            return null;

        return current_selected_slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();
    }

    private void UpdateToolMarker()
    {
        if (!IsToolMarkerValid())
            return;

        Vector2 mouse_world_pos = GetMouseWorldPosition();
        Island nearest_island = IslandManager.instance.GetNearestIsland(mouse_world_pos);

        if (!IsValidFarmland(nearest_island))
        {
            HideToolMarker();
            return;
        }

        Vector2I tile_under_mouse = GetTileUnderMouse(nearest_island, mouse_world_pos);
        Vector2 tile_center_global = GetTileCenterGlobalPosition(nearest_island, tile_under_mouse);

        tool_marker_container.GlobalPosition = tile_center_global;
        tool_marker_container.Visible = true;
    }

    private bool IsToolMarkerValid()
    {
        return tool_marker != null && tool_marker_texture != null;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Camera2D camera = GetViewport().GetCamera2D();
        return camera != null ? camera.GetGlobalMousePosition() : GetGlobalMousePosition();
    }

    private Vector2I GetTileUnderMouse(Island island, Vector2 mouse_world_pos)
    {
        Vector2 mouse_tilemap_local_pos = island.farmland_tilemap.ToLocal(mouse_world_pos);
        return island.farmland_tilemap.LocalToMap(mouse_tilemap_local_pos);
    }

    private Vector2 GetTileCenterGlobalPosition(Island island, Vector2I tile_pos)
    {
        Vector2 tile_center_local = island.farmland_tilemap.MapToLocal(tile_pos);
        return island.farmland_tilemap.ToGlobal(tile_center_local);
    }

    private bool IsValidFarmland(Island island)
    {
        return island != null && island.farmland_tilemap != null;
    }

    private bool IsValidIsland(Island island)
    {
        if (island == null)
        {
            Debug.Print("[DEBUG] No island found under mouse");
            return false;
        }

        if (island.farmland_tilemap == null)
        {
            Debug.Print("[DEBUG] Island has no farmland tilemap");
            return false;
        }

        return true;
    }

    private bool HasFarmlandTile(Island island, Vector2I tile_pos)
    {
        if (island.farmland_tilemap.GetCellSourceId(tile_pos) < 0)
        {
            Debug.Print($"[DEBUG] No farmland tile at position {tile_pos}");
            return false;
        }

        return true;
    }

    private bool HasValidToolAttribute()
    {
        if (current_tool_attr == null || current_tool_attr.auto_tile_id < 0)
        {
            Debug.Print("[DEBUG] Tool has no valid farmland tile source ID");
            return false;
        }

        return true;
    }

    private bool IsModificationCooldownExpired()
    {
        double current_time = Time.GetTicksMsec() / 1000.0;
        return current_time - last_farmland_modification_time >= FARMLAND_MODIFICATION_COOLDOWN;
    }

    private bool CanRemoveTile()
    {
        if (!CheckBuildingColliders(isRemoving: true))
        {
            Debug.Print("[DEBUG] Cannot remove tile - collision check failed");
            return false;
        }

        return true;
    }

    private bool CanPlaceTile()
    {
        if (!CheckBuildingColliders())
        {
            Debug.Print("[DEBUG] Cannot place tile - collision check failed");
            return false;
        }

        return true;
    }

    private bool CheckBuildingColliders(bool isRemoving = false)
    {
        if (Logger.NodeIsNull(tool_marker_collider_manager) || Logger.NodeIsNull(current_tool_attr))
            return false;

        Array<placeable_building.TILETYPE> tileTypes = isRemoving
            ? current_tool_attr.can_be_removed_on_tile_types
            : current_tool_attr.can_be_used_on_tile_types;

        tool_marker_collider_manager.SetTileType(tileTypes);
        return tool_marker_collider_manager.AllCollidersOnBuildingLayer(null);
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
            farmland.SetCellsTerrainConnect(new Array<Vector2I> { tile_pos }, 0, -1);
            Debug.Print($"[DEBUG ModifyFarmlandTile] Tile erased at position {tile_pos}");
        }
        else
        {
            Debug.Print($"[DEBUG ModifyFarmlandTile] No tile at position {tile_pos}");
        }
    }

    private void PlaceFarmlandTile(Island island, Vector2I tile_pos)
    {
        island.farmland_tilemap.SetCellsTerrainConnect(
            new Array<Vector2I> { tile_pos },
            0,
            current_tool_attr.auto_tile_id
        );

        var set_source_id = island.farmland_tilemap.GetCellSourceId(tile_pos);
        var set_atlas = island.farmland_tilemap.GetCellAtlasCoords(tile_pos);
        Debug.Print(
            $"[DEBUG] After SetCellsTerrainConnect - Source ID: {set_source_id}, Atlas: {set_atlas}"
        );
    }

    private void ReduceDurability()
    {
        if (Logger.NodeIsNotNull(EquipmentPanel.instance))
            EquipmentPanel.instance.RemoveDurability(1);
    }

    private void UpdateModificationTime()
    {
        last_farmland_modification_time = Time.GetTicksMsec() / 1000.0;
    }
}
