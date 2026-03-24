using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class MonsterIsland : Building_Node
{
    public static MonsterIsland instance;

    [Export]
    public Array<Texture2D> state_textures = new Array<Texture2D>();
    public static float nerv_normal = 1f;
    public static float nerv_abuse = 0f;

    private MonsterIslandStateManager island_state;
    public SpriteAnimationManager sprite_animation_manager;
    private Resource cutscene_item = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://bcvaepfvv50ax")
    );
    private bool monsterDisappearTriggered = false;
    private bool monsterAppearTriggered = false;

    private int quest_duration = 0;
    public HealthBar health_bar;
    private HitLabelManager hit_label_manager;

    [Signal]
    public delegate void VisibilityIncreasedEventHandler();

    [Signal]
    public delegate void HitLabelManagerFinishedEventHandler();

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {
        base._Ready();
        sprite_animation_manager = GetNode<SpriteAnimationManager>("SpriteAnimationManager");
        island_state = GetNode<MonsterIslandStateManager>("StateManager");
        health_bar = GetNode<HealthBar>("HealthBar");
        hit_label_manager = GetNode<HitLabelManager>("HitLabelManager");
        island_state.StateChanged += OnStateChanged;
        UpdateStateVisuals(island_state.GetCurrentState());
    }

    public float GetNervRate()
    {
        float rate = nerv_normal + NervTransducterManager.instance.GetNervReduction() - nerv_abuse;
        if (rate > 1f)
            rate = 1f;
        else if (rate < 0f)
            rate = 0f;

        return rate;
    }

    public void InitializeQuestTimers()
    {
        quest_duration = QuestManager.instance.quests[QuestManager.current_quest_id].quest_time;
        monsterDisappearTriggered = false;
        monsterAppearTriggered = false;
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
            StartHittingCutscene();

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

        // Monster verschwindet
        if (!monsterDisappearTriggered && quest_time_left <= (quest_duration - 240))
        {
            monsterDisappearTriggered = true;
            is_visible = false;
            CutsceneManager.instance.QueueCutscene(cutscene_item, "Monster_Disappear");
        }

        // Monster erscheint
        if (!monsterAppearTriggered && quest_time_left <= 240)
        {
            monsterAppearTriggered = true;
            is_visible = true;
            CutsceneManager.instance.QueueCutscene(cutscene_item, "Monster_Appear");
        }
    }

    public void StartHittingCutscene()
    {
        CutsceneManager.instance.QueueCutscene(cutscene_item, "Monster_Hitting");
    }

    public async Task HitIsland(int amount, int hits)
    {
        for (int x = 0; x < hits; x++)
        {
            hit_label_manager.ShowHitLabel(amount);
            health_bar.RemoveHealth(amount);
            await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        }
    }

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 45)
            return;

        GameMenu.questMenu.OnOpenQuestMenu();
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

    public void SetMood(float value)
    {
        island_state.SetMood(value);
    }

    public void SetStability(float value)
    {
        island_state.SetStability(value);
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
