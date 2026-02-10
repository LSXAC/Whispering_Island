using System;
using Godot;
using Godot.Collections;

public partial class MonsterIslandStateManager : Node
{
    [Export]
    public float quest_completed_mood_bonus = 0.15f;

    [Export]
    public float quest_failed_mood_penalty = -0.20f;

    [Export]
    public float quest_overfulfilled_stability_penalty = -0.1f;

    [Export]
    public float escalation_stability_penalty = -0.10f;

    [Export]
    public float manipulation_stability_penalty = 0.1f;

    public enum STATE
    {
        CALM,
        IRRITATED,
        ANGRY,
        UNSTABLE,
        COLLAPSE,
        HAPPY
    }

    private STATE current_state = STATE.CALM;
    private float mood = 1.0f;
    private float stability = 1.0f;

    public event Action<STATE> StateChanged;

    public override string ToString()
    {
        return $"State: {current_state}, Mood: {mood:P0}, Stability: {stability:P0}";
    }

    public override async void _Ready()
    {
        await PlayerUI.instance.ToSignal(PlayerUI.instance, PlayerUI.SignalName.Loaded);
        PlayerUI.instance.monster_island_state_panel.UpdateMoodItem(current_state);
        PlayerUI.instance.monster_island_state_panel.UpdateStabiltyItem(stability);
    }

    public void ApplyQuestCompleted()
    {
        mood += quest_completed_mood_bonus;
        mood = Mathf.Clamp(mood, 0f, 1f);
        UpdateState();
    }

    public void ApplyQuestFailed()
    {
        mood += quest_failed_mood_penalty;
        mood = Mathf.Clamp(mood, 0f, 1f);
        UpdateState();
    }

    public void ApplyQuestOverfulfilled()
    {
        stability += quest_overfulfilled_stability_penalty;
        stability = Mathf.Clamp(stability, 0f, 1f);
        UpdateState();
    }

    public void ApplyEscalation()
    {
        stability += escalation_stability_penalty;
        stability = Mathf.Clamp(stability, 0f, 1f);
        UpdateState();
    }

    public void ApplyManipulation()
    {
        stability += manipulation_stability_penalty;
        stability = Mathf.Clamp(stability, 0f, 1f);
        UpdateState();
    }

    private void UpdateState()
    {
        STATE new_state = current_state;

        // Collapse takes priority
        if (stability < 0.1f)
        {
            new_state = STATE.COLLAPSE;
        }
        // Unstable state
        else if (stability < 0.4f)
        {
            new_state = STATE.UNSTABLE;
        }
        // Angry state
        else if (mood < 0.15f)
        {
            new_state = STATE.ANGRY;
        }
        // Irritated state
        else if (mood < 0.4f)
        {
            new_state = STATE.IRRITATED;
        }
        // Calm state
        else if (mood >= 0.7f && stability >= 0.7f)
        {
            new_state = STATE.CALM;
        }
        // Default to current state if no conditions met
        else if (mood >= 0.4f)
        {
            new_state = STATE.IRRITATED;
        }

        if (new_state != current_state)
        {
            current_state = new_state;
            StateChanged?.Invoke(current_state);
            PlayerUI.instance.monster_island_state_panel.UpdateMoodItem(current_state);
            PlayerUI.instance.monster_island_state_panel.UpdateStabiltyItem(stability);
        }
    }

    public STATE GetCurrentState()
    {
        return current_state;
    }

    public float GetMood()
    {
        return mood;
    }

    public float GetStability()
    {
        return stability;
    }
}
