using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildingCollider : Area2D
{
    [Export]
    public bool on_building_layer = false;

    [Export]
    private Texture2D grid_outline_green;

    [Export]
    private Texture2D grid_outline_red;

    private TextureRect rect;
    public Array<placeable_building.TILETYPE> types;
    private bool activated = true;
    public BuildingColliderManager.PLACE_TYPE type = BuildingColliderManager.PLACE_TYPE.BUILDING;

    public override void _Ready()
    {
        rect = GetNode<TextureRect>("TextureRect");
        if (Logger.NodeIsNull(rect))
            return;

        SetRectVisible(false);
    }

    public override void _Process(double delta)
    {
        if (
            Logger.NodeIsNull(rect)
            || Logger.NodeIsNull(grid_outline_green)
            || Logger.NodeIsNull(grid_outline_red)
        )
            return;

        if (IsCurrentBuilding())
            SetRectVisible(true);
        else if (rect.Texture == grid_outline_green)
            SetRectVisible(false);
    }

    public void OnBodyEntered(Node2D node) => Calc();

    public void OnBodyLeaved(Node2D node) => Calc();

    public void Calc(bool on_air = false)
    {
        bool found = false;
        foreach (Node2D node in GetOverlappingBodies())
        {
            if (type == BuildingColliderManager.PLACE_TYPE.MOVEABLE)
            {
                if (node is RailArea)
                {
                    found = true;
                    BuildingPlacer.moveable_selected_parent = node.GetParent<Rail>()?.item_holder;
                    break;
                }
                continue;
            }

            if (node is Building_Node building_node && !building_node.disable_collision)
            {
                Debug.Print("Building Node in the way!");
                found = false;
                break;
            }

            if (node is StaticBody2D && node.Name.ToString().Contains("Bridge"))
            {
                Debug.Print("Static Body is in the way!");
                found = false;
                break;
            }

            if (on_air)
            {
                found = true;
                break;
            }
            if (node.IsInGroup("BLOCKING"))
            {
                Debug.Print("Blocking Node is in the way!");
                found = false;
                break;
            }

            if (types != null)
            {
                foreach (var t in types)
                {
                    if (node.IsInGroup(t.ToString()))
                    {
                        Debug.Print("Right Layer!");
                        found = true;
                    }
                }
            }
        }

        on_building_layer = found;
        rect.Texture = found ? grid_outline_green : grid_outline_red;
    }

    private void SetRectVisible(bool state)
    {
        activated = state;
        if (rect != null)
            rect.Visible = state;
    }

    private bool IsCurrentBuilding()
    {
        return BuildingPlacer.current_building == GetParent()?.GetParent();
    }
}
