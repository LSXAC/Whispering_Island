using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class IslandMenu : ColorRect
{
    [Export]
    public IslandMenuItem[] islands;

    [Export]
    private Control island_parent;

    private Array<IslandMenuItem> island_menu_items = new Array<IslandMenuItem>();

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

        foreach (Control c in island_parent.GetChildren())
            if (c is IslandMenuItem menu_item)
                island_menu_items.Add(menu_item);

        island_menu_items[0].buy_btn.Pressed += () => SelectIsland(0);
        island_menu_items[1].buy_btn.Pressed += () => SelectIsland(1);
        island_menu_items[2].buy_btn.Pressed += () => SelectIsland(2);
        island_menu_items[3].buy_btn.Pressed += () => SelectIsland(3);
    }

    public void OnVisiblityChanged()
    {
        island_to_build_left_label.Text =
            TranslationServer.Translate("ISLAND_MENU_PRE_ISLANDSBUILD")
            + ": "
            + (GameManager.GetIslandCountOnMatrix() - 1)
            + " / "
            + (GameManager.island_matrix.Length - 1)
            + " "
            + TranslationServer.Translate("ISLAND_MENU_AFTER_ISLANDSBUILD");
        DisableButtons();
        GetTree().CreateTimer(0.2f).Timeout += () => SetButtons();
    }

    public void SetButtons()
    {
        for (int i = 0; i < island_menu_items.Count; i++)
        {
            island_menu_items[i]
                .UpdateMoneyLabel(
                    (base_cost * (IslandManager.instance.island_types_build[i] + 1)).ToString()
                );
            island_menu_items[i]
                .UpdateTitle(
                    TranslationServer.Translate(
                        "ISLAND_MENU_" + island_menu_items[i].item_menu_item_data.menu_item_title
                    )
                );
            if (GameManager.money >= base_cost * (IslandManager.instance.island_types_build[i] + 1))
                island_menu_items[i].buy_btn.Disabled = false;
        }
    }

    public void DisableButtons()
    {
        for (int i = 0; i < island_menu_items.Count; i++)
        {
            island_menu_items[i].buy_btn.Disabled = true;
        }
    }

    public void SelectIsland(int id)
    {
        if (current_sign == null)
            return;
        Debug.Print("Selected Island ID: " + id.ToString());

        GameManager.money -= base_cost * (IslandManager.instance.island_types_build[id] + 1);
        IslandManager.instance.island_types_build[id]++;

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
        Debug.Print("Get Island Dir.");
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

        Debug.Print("Found Island Dir.");
        current_ip.CreateAnotherIsland(
            islands[unique_id].item_menu_item_data.island_scene,
            dir,
            is_loading
        );
    }
}
