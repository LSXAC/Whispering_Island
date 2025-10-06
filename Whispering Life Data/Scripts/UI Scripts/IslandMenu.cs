using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public partial class IslandMenu : ColorRect
{
    private PackedScene mystic_island = ResourceLoader.Load<PackedScene>(
        "res://Scenes/Islands/mystical_island.tscn"
    );
    private PackedScene mining_island = ResourceLoader.Load<PackedScene>(
        "res://Scenes/Islands/mining_island.tscn"
    );
    private PackedScene farming_island = ResourceLoader.Load<PackedScene>(
        "res://Scenes/Islands/farming_island.tscn"
    );
    private PackedScene dessert_island = ResourceLoader.Load<PackedScene>(
        "res://Scenes/Islands/dessert_island.tscn"
    );

    [Export]
    private Button[] buttons;

    [Export]
    public Label[] island_cost;

    [Export]
    public Label island_to_build_left_label;

    [Export]
    public int base_cost = 10;

    public enum ISLANDS
    {
        ISLAND_0,
        ISLAND_1,
        ISLAND_2,
        ISLAND_3
    };

    public Sign current_sign = null;
    public static IslandMenu instance;

    public override void _Ready()
    {
        instance = this;
        buttons[0].Pressed += () => SelectIsland(0);
        buttons[1].Pressed += () => SelectIsland(1);
        buttons[2].Pressed += () => SelectIsland(2);
        buttons[3].Pressed += () => SelectIsland(3);
    }

    public void OnVisiblityChanged()
    {
        island_to_build_left_label.Text =
            TranslationServer.Translate("ISLAND_MENU_PRE_ISLANDSBUILD")
            + ": "
            + (GameManager.GetIslandCountOnMatrix() - 1)
            + " / "
            + (GameManager.island_matrix.Length - 1)
            + TranslationServer.Translate("ISLAND_MENU_AFTER_ISLANDSBUILD");
        DisableButtons();
        GetTree().CreateTimer(0.2f).Timeout += () => SetButtons();
    }

    public void SetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            island_cost[i].Text = (
                base_cost * (IslandManager.instance.island_types_build[i] + 1)
            ).ToString();
            if (GameManager.money >= base_cost * (IslandManager.instance.island_types_build[i] + 1))
                buttons[i].Disabled = false;
        }
    }

    public void DisableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Disabled = true;
        }
    }

    public void SelectIsland(int id)
    {
        if (current_sign == null)
            return;
        Debug.Print("Selected Island ID: " + id.ToString());
        IslandManager.instance.island_types_build[id]++;

        GameManager.money -= base_cost * (IslandManager.instance.island_types_build[id] + 1);
        PlayerUI.instance.UpdateMoneyLabel();

        CreateIsland(id, current_sign.dir, current_sign.island);

        GameMenu.instance.OnCloseIslandTab();
        current_sign = null;
    }

    public void CreateIsland(
        int unique_id,
        Island.DIRECTION dir,
        Island current_ip,
        bool is_loading = false
    )
    {
        Debug.Print("Get Dir.");
        if (dir == Island.DIRECTION.UP)
            if (GameManager.IsIslandOnMatrix(current_ip.matrix_x, current_ip.matrix_y - 1))
                return;
        if (dir == Island.DIRECTION.DOWN)
            if (GameManager.IsIslandOnMatrix(current_ip.matrix_x, current_ip.matrix_y + 1))
                return;
        if (dir == Island.DIRECTION.LEFT)
            if (GameManager.IsIslandOnMatrix(current_ip.matrix_x - 1, current_ip.matrix_y))
                return;
        if (dir == Island.DIRECTION.RIGHT)
            if (GameManager.IsIslandOnMatrix(current_ip.matrix_x + 1, current_ip.matrix_y))
                return;

        Debug.Print("Found Dir.");
        switch (unique_id)
        {
            case 0:
                Debug.Print("Island 0.");
                current_ip.CreateAnotherIsland(mystic_island, dir, is_loading);
                break;
            case 1:
                Debug.Print("Island 1.");
                current_ip.CreateAnotherIsland(mining_island, dir, is_loading);
                break;
            case 2:
                Debug.Print("Island 2.");
                current_ip.CreateAnotherIsland(farming_island, dir, is_loading);
                break;
            case 3:
                Debug.Print("Island 3.");
                current_ip.CreateAnotherIsland(dessert_island, dir, is_loading);
                break;
            default:
                Debug.Print("No matching Island ID found");
                break;
        }
    }
}
