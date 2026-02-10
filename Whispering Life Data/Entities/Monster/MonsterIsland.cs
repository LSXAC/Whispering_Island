using System;
using Godot;
using Godot.Collections;

public partial class MonsterIsland : Building_Node
{
    public static MonsterIsland instance;

    [Export]
    public Array<Texture2D> state_textures = new Array<Texture2D>();

    private MonsterIslandStateManager island_state;
    private SpriteAnimationManager sprite_animation_manager;

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

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.F1))
            island_state.ApplyQuestCompleted();

        if (Input.IsKeyPressed(Key.F2))
            island_state.ApplyQuestFailed();

        if (Input.IsKeyPressed(Key.F3))
            island_state.ApplyManipulation();

        if (Input.IsKeyPressed(Key.F4))
            island_state.ApplyEscalation();
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
