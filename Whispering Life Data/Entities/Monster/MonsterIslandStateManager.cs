using System;
using Godot;
using Godot.Collections;

public partial class MonsterIslandStateManager : Node
{
    [Export]
    public float quest_completed_mood_bonus = 0.10f;

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
    private float mood = 0.8f;
    private float stability = 1.0f;

    public event Action<STATE> StateChanged;

    public override string ToString()
    {
        return $"State: {current_state}, Mood: {mood:P0}, Stability: {stability:P0}";
    }

    public override async void _Ready()
    {
        await PlayerUI.instance.ToSignal(PlayerUI.instance, PlayerUI.SignalName.Loaded);
        UpdateState();
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

        // Check if mood reached 0 - trigger game over
        if (mood <= 0f)
        {
            GameManager.instance.GameOver();
            return;
        }

        // States based on mood in 0.2 steps:
        // 0.0 - 0.2: COLLAPSE
        // 0.2 - 0.4: ANGRY
        // 0.4 - 0.6: IRRITATED
        // 0.6 - 0.8: CALM
        // 0.8 - 1.0: HAPPY
        if (mood < 0.2f)
        {
            new_state = STATE.COLLAPSE;
        }
        else if (mood < 0.4f)
        {
            new_state = STATE.ANGRY;
        }
        else if (mood < 0.6f)
        {
            new_state = STATE.IRRITATED;
        }
        else if (mood < 0.8f)
        {
            new_state = STATE.CALM;
        }
        else
        {
            new_state = STATE.HAPPY;
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

    public void SetMood(float value)
    {
        mood = value;
    }

    public void SetStability(float value)
    {
        stability = value;
    }
}
