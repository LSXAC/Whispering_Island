using System;
using System.Diagnostics;
using Godot;

public partial class MouseArea : Area2D
{
    [Export]
    private Building_Node building_node;

    [Export]
    public Sprite2D building_sprite;

    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Outline_Shader.tres"
    );
    private ShaderMaterial remove_outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Remove_Outline_Color.tres"
    );

    private void OutlineBuilding()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.None)
        {
            if (GetParent() is ResourceObject)
                if (((ResourceObject)GetParent()).in_cooldown)
                    return;

            building_sprite.Material = outline_shader;
            building_node.mouse_inside = true;
            if (!building_node.GetTitle().ToString().ToUpper().Contains("BELT"))
            {
                hover_menu.InitHoverMenu(building_node);
                Debug.Print("Hover Menu");
            }
        }
        else if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
        {
            building_sprite.Material = remove_outline_shader;
            building_node.mouse_inside = true;
        }
    }

    public void OnMouseEntered()
    {
        if (building_node == null || Game_Manager.In_Cutscene)
        {
            return;
        }
        OutlineBuilding();
    }

    public void OnMouseClick()
    {
        if (
            Game_Manager.inside_game_menu
            || Game_Manager.building_mode != Game_Manager.BuildingMode.None
            || building_node == null
        )
            return;

        if (!building_node.mouse_inside)
            return;

        if (Global.GetDistanceToPlayer(this.GlobalPosition) >= 40f)
            return;

        if (GetParent().HasNode("Actionable"))
            GetParent().GetNode<Actionable>("Actionable").Action();

        if (GetParent() is ProcessBuilding)
        {
            FurnaceTab.INSTANCE.SetProcessBuilding(GetParent<ProcessBuilding>());
            GameMenu.INSTANCE.OnOpenFurnaceTab();
        }

        if (GetParent() is Chest)
        {
            GameMenu.INSTANCE.OnOpenChestTab();
            ChestInventory.INSTANCE.OpenChest(GetParent<Chest>());
            Debug.Print("CHEST!" + " | " + Name);
        }

        if (GetParent() is Bed)
        {
            //Open Bed UI, Select Time you want to Sleep
            Player.INSTANCE.player_stats.RemoveFatigue(seconds: 5);
            Debug.Print("Sleep");
        }
        if (GetParent() is ResearchTable)
        {
            GameMenu.INSTANCE.OnOpenResearchTab();
        }
    }

    public void OnMouseLeaved()
    {
        building_sprite.Material = null;
        if (building_node != null)
        {
            building_node.mouse_inside = false;
            hover_menu.DisableHoverMenu();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.Pressed)
            {
                OnMouseClick();
            }
    }
}
