using Godot;
using Godot.Collections;

public partial class GlobalFunctions : Node2D
{
    public static GlobalFunctions instance;
    public static float transport_moving_speed = 10f;

    public override void _Ready()
    {
        instance = this;
    }

    public static int RoundToNextInt(float id)
    {
        return (int)(id + 0.5f);
    }

    public static float GetTransportMovingSpeed()
    {
        return transport_moving_speed;
    }

    public static Array<Item> GetNormalListOrDevList(Array<Item> items)
    {
        if (GameManager.dev_build_mode)
            return new Array<Item>()
            {
                new Item(Inventory.ITEM_TYPES[Inventory.ITEM_ID.OAK_WOOD], 1)
            };
        return items;
    }

    public static bool HasResearchLevel(Inventory.ITEM_ID id, Database.UPGRADE_LEVEL level)
    {
        if (ResearchTab.research_saves.ContainsKey(id))
            if (ResearchTab.research_saves[id].research_level >= (int)level)
                return true;

        return false;
    }

    public static void NextQuestHalfTime()
    {
        QuestManager.next_quest_half_time = true;
        OpenAcceptPanel();
        PlayerUI.instance.quest_accept_panel.InitAcceptPanel(QuestAcceptPanel.PENEALTY.HALF_TIME);
    }

    public static void NextQuestDoubledItems()
    {
        OpenAcceptPanel();
        PlayerUI.instance.quest_accept_panel.InitAcceptPanel(
            QuestAcceptPanel.PENEALTY.DOUBLE_AMOUNT
        );
    }

    public static void NextQuestIslandsRemoved()
    {
        OpenAcceptPanel();
        PlayerUI.instance.quest_accept_panel.InitAcceptPanel(
            QuestAcceptPanel.PENEALTY.ISLAND_REMOVE
        );
    }

    public static void OpenAcceptPanel()
    {
        GameManager.instance.cutscene_camera.Enabled = false;
        Player.camera.Position = GameManager.instance.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Vector2.Zero;
        PlayerUI.instance.quest_accept_panel.Visible = true;
    }

    public static void SetPlayerPositionToStart()
    {
        GameManager.instance.save_state.char_save.player_position = new Vector2(10f, -170f);
    }

    public static bool CheckResearchRequirements(Array<UnlockRequirement> br)
    {
        foreach (UnlockRequirement temp in br)
        {
            if (ResearchTab.instance == null)
                return false;
            if (!ResearchTab.research_saves.ContainsKey(temp.item_id))
                return false;
            if (ResearchTab.research_saves[temp.item_id].research_level < (int)temp.required_level)
                return false;
        }
        return true;
    }

    public static float GetDistanceToPlayer(Vector2 nodePosition)
    {
        return Player.instance.GlobalPosition.DistanceTo(nodePosition);
    }

    public static void MoveCameraToPosition(Vector2 pos)
    {
        Player.camera.Enabled = false;
        GameManager.instance.cutscene_camera.GlobalPosition = Player.camera.GlobalPosition;
        GameManager.instance.cutscene_camera.Enabled = true;
        GameManager.instance.cutscene_camera.Position = pos;
    }

    public static void StartAfterTutorial()
    {
        GameManager.instance.tutorial_finished = true;
        TimeManager.instance.game_timer.Start();
        QuestManager.instance.StartQuest();
    }

    public static void InDialogue()
    {
        GameManager.In_Cutscene = true;
    }

    public static void QueueFreeTree()
    {
        Tutorial.instance.Tree.QueueFree();
    }

    public static void LeaveDialogue()
    {
        GameManager.instance.cutscene_camera.Enabled = false;
        Player.camera.Position = GameManager.instance.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Vector2.Zero;
        GameManager.In_Cutscene = false;
        GameManager.instance.SaveGame();
    }

    public static void OutlineTree()
    {
        Tutorial.instance.Tree.Visible = true;
    }

    public static void RemoveOutlineTree()
    {
        Tutorial.instance.Tree.Visible = false;
    }
}
