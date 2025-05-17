using Godot;
using Godot.Collections;

public partial class GlobalFunctions : Node2D
{
    public static GlobalFunctions INSTANCE;
    public static float transport_moving_speed = 10f;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public static int RoundToNextInt(float id)
    {
        return (int)(id + 0.5f);
    }

    public static float GetTransportMovingSpeed()
    {
        return transport_moving_speed;
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
        player_ui.INSTANCE.quest_accept_panel.InitAcceptPanel(QuestAcceptPanel.PENEALTY.HALF_TIME);
    }

    public static void NextQuestDoubledItems()
    {
        OpenAcceptPanel();
        player_ui.INSTANCE.quest_accept_panel.InitAcceptPanel(
            QuestAcceptPanel.PENEALTY.DOUBLE_AMOUNT
        );
    }

    public static void NextQuestIslandsRemoved()
    {
        OpenAcceptPanel();
        player_ui.INSTANCE.quest_accept_panel.InitAcceptPanel(
            QuestAcceptPanel.PENEALTY.ISLAND_REMOVE
        );
    }

    public static void OpenAcceptPanel()
    {
        Game_Manager.INSTANCE.cutscene_camera.Enabled = false;
        Player.camera.Position = Game_Manager.INSTANCE.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Vector2.Zero;
        player_ui.INSTANCE.quest_accept_panel.Visible = true;
    }

    public static void SetPlayerPositionToStart()
    {
        Game_Manager.INSTANCE.save_state.char_save.player_position = new Vector2(10f, -170f);
    }

    public static bool CheckResearchRequirements(Array<UnlockRequirement> br)
    {
        foreach (UnlockRequirement temp in br)
        {
            if (ResearchTab.INSTANCE == null)
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
        return Player.INSTANCE.GlobalPosition.DistanceTo(nodePosition);
    }

    public static void MoveCameraToPosition(Vector2 pos)
    {
        Player.camera.Enabled = false;
        Game_Manager.INSTANCE.cutscene_camera.GlobalPosition = Player.camera.GlobalPosition;
        Game_Manager.INSTANCE.cutscene_camera.Enabled = true;
        Game_Manager.INSTANCE.cutscene_camera.Position = pos;
    }

    public static void StartAfterTutorial()
    {
        Game_Manager.INSTANCE.tutorial_finished = true;
        Game_Manager.INSTANCE.game_timer.Start();
        QuestManager.INSTANCE.StartQuest();
    }

    public static void InDialogue()
    {
        Game_Manager.In_Cutscene = true;
    }

    public static void QueueFreeTree()
    {
        Tutorial.INSTANCE.Tree.QueueFree();
    }

    public static void LeaveDialogue()
    {
        Game_Manager.INSTANCE.cutscene_camera.Enabled = false;
        Player.camera.Position = Game_Manager.INSTANCE.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Vector2.Zero;
        Game_Manager.In_Cutscene = false;
        Game_Manager.INSTANCE.SaveGame();
    }

    public static void OutlineTree()
    {
        Tutorial.INSTANCE.Tree.Visible = true;
        Tutorial.INSTANCE.shadow.Visible = true;
        Tutorial.INSTANCE.Tree.Material = Tutorial.INSTANCE.outline_shader;
    }

    public static void RemoveOutlineTree()
    {
        Tutorial.INSTANCE.Tree.Material = null;
        Tutorial.INSTANCE.Tree.Visible = false;
        Tutorial.INSTANCE.shadow.Visible = false;
    }
}
