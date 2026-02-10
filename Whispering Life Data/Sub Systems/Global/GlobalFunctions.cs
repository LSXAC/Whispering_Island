using System.Threading.Tasks;
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

    public static async Task IncreaseVisibilityOnMonsterIsland()
    {
        var animationPlayer = MonsterIsland.instance.sprite_animation_manager.GetAnimationPlayer();
        MonsterIsland.instance.IncreaseVisibility();
        await MonsterIsland.instance.ToSignal(
            animationPlayer,
            AnimationPlayer.SignalName.AnimationFinished
        );
    }

    public static void PlayIdleOnMonsterIsland()
    {
        MonsterIsland.instance.PlayIdle();
    }

    public static async Task DecreaseVisibilityOnMonsterIsland()
    {
        var animationPlayer = MonsterIsland.instance.sprite_animation_manager.GetAnimationPlayer();
        MonsterIsland.instance.DecreaseVisibility();
        await MonsterIsland.instance.ToSignal(
            animationPlayer,
            AnimationPlayer.SignalName.AnimationFinished
        );
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
        CutsceneManager.instance.cutscene_camera.Enabled = false;
        Player.camera.Position = CutsceneManager.instance.cutscene_camera.Position;
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

    public static bool HasItemsInInventory(Array<Item> required_items)
    {
        foreach (Item quest_item in required_items)
        {
            Array<Item> iii = PlayerInventoryUI.instance.GetItemFromListOrNull(
                PlayerInventoryUI.instance.GetListOfItemsInInventory(),
                quest_item
            );

            if (iii == null)
                return false;

            int amount = 0;
            foreach (Item i_x in iii)
                amount += i_x.amount;

            if (QuestManager.next_quest_is_doubled_items)
            {
                if (amount < quest_item.amount * 2)
                    return false;
            }
            else if (amount < quest_item.amount)
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
        CutsceneManager.instance.cutscene_camera.GlobalPosition = Player.camera.GlobalPosition;
        CutsceneManager.instance.cutscene_camera.Enabled = true;
        CutsceneManager.instance.cutscene_camera.Position = pos;
    }

    public static void StartAfterTutorial()
    {
        GameManager.instance.tutorial_finished = true;
        TimeManager.instance.game_timer.Start();
        QuestManager.instance.StartQuest();
    }

    public static void InDialogue()
    {
        CutsceneManager.In_Cutscene = true;
    }

    public static void QueueFreeTree()
    {
        Tutorial.instance.Tree.QueueFree();
    }

    public static void LeaveDialogue()
    {
        CutsceneManager.instance.cutscene_camera.Enabled = false;
        Player.camera.Position = CutsceneManager.instance.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Vector2.Zero;
        CutsceneManager.In_Cutscene = false;
        CutsceneManager.instance.EmitSignal(CutsceneManager.SignalName.CutsceneFinished);
        GameManager.instance.SaveGame();
    }

    public static void OutlineTree()
    {
        Tutorial.instance.Tree.Visible = true;
        Tutorial.instance.Tree.shadowNode.shadow_sprite.Visible = true;
    }

    public static void RemoveOutlineTree()
    {
        Tutorial.instance.Tree.Visible = false;
        Tutorial.instance.Tree.shadowNode.RemoveShadow();
    }
}
