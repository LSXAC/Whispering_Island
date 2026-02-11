using System;
using Godot;
using Godot.Collections;

public partial class MonsterIsland : Building_Node
{
    public static MonsterIsland instance;

    [Export]
    public Array<Texture2D> state_textures = new Array<Texture2D>();

    private MonsterIslandStateManager island_state;
    public SpriteAnimationManager sprite_animation_manager;
    private Resource cutscene_item = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://bcvaepfvv50ax")
    );

    private int quest_duration = 0;

    [Signal]
    public delegate void VisibilityIncreasedEventHandler();

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {
        base._Ready();
        sprite_animation_manager = GetNode<SpriteAnimationManager>("SpriteAnimationManager");
        island_state = GetNode<MonsterIslandStateManager>("StateManager");
        island_state.StateChanged += OnStateChanged;
        UpdateStateVisuals(island_state.GetCurrentState());
    }

    public void InitializeQuestTimers()
    {
        quest_duration = QuestManager.instance.quests[QuestManager.current_quest_id].quest_time;
        is_visible = true;
    }

    public void IncreaseVisibility()
    {
        sprite_animation_manager.PlayAnimation("Increase");
        is_visible = true;
    }

    public void DecreaseVisibility()
    {
        sprite_animation_manager.PlayAnimation("Decrease");
        is_visible = false;
    }

    public void PlayIdle()
    {
        sprite_animation_manager.PlayAnimation("Idle");
    }

    public override void _Process(double delta)
    {
        // Cutscene-Trigger für Quest-Zeitpunkte
        CheckQuestTimeTriggers();

        if (Input.IsKeyPressed(Key.F1))
            island_state.ApplyQuestCompleted();

        if (Input.IsKeyPressed(Key.F2))
            island_state.ApplyQuestFailed();

        if (Input.IsKeyPressed(Key.F3))
            island_state.ApplyManipulation();

        if (Input.IsKeyPressed(Key.F4))
            island_state.ApplyEscalation();
    }

    private void CheckQuestTimeTriggers()
    {
        int quest_time_left = QuestManager.current_quest_time;

        // Monster verschwindet: wenn verbrauchte Zeit >= 3h
        if (is_visible && quest_time_left <= (quest_duration - 180))
        {
            is_visible = false;
            CutsceneManager.instance.QueueCutscene(cutscene_item, "Monster_Disappear");
        }

        // Monster kommt zurück: wenn noch 3h verbleiben
        if (!is_visible && quest_time_left <= 180)
        {
            is_visible = true;
            CutsceneManager.instance.QueueCutscene(cutscene_item, "Monster_Appear");
        }
    }

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 45)
            return;

        QuestMenu.instance.OnOpenQuestMenu();
    }

    private void OnStateChanged(MonsterIslandStateManager.STATE new_state)
    {
        UpdateStateVisuals(new_state);
    }

    private void UpdateStateVisuals(MonsterIslandStateManager.STATE state)
    {
        int state_index = (int)state;
        if (state_index < state_textures.Count && state_textures[state_index] != null)
            sprite_animation_manager.SetTexture2D(state_textures[state_index]);
        else
            GD.PrintErr($"Texture not found for state: {state}");
    }

    public MonsterIslandStateManager.STATE GetCurrentState()
    {
        return island_state.GetCurrentState();
    }

    public float GetMood()
    {
        return island_state.GetMood();
    }

    public float GetStability()
    {
        return island_state.GetStability();
    }

    public void ApplyQuestCompleted()
    {
        island_state.ApplyQuestCompleted();
    }

    public void ApplyQuestFailed()
    {
        island_state.ApplyQuestFailed();
    }

    public void ApplyQuestOverfulfilled()
    {
        island_state.ApplyQuestOverfulfilled();
    }

    public void ApplyEscalation()
    {
        island_state.ApplyEscalation();
    }

    public void ApplyManipulation()
    {
        island_state.ApplyManipulation();
    }
}
