using System;
using System.Diagnostics;
using Godot;

public partial class TransitionManager : CanvasLayer
{
    [Signal]
    public delegate void InTransitionEventHandler();

    [Signal]
    public delegate void TransitionEndedCompletlyEventHandler();

    [Export]
    public AnimationPlayer anim_player;

    public static TransitionManager INSTANCE = null;
    public static STATE current_state = STATE.START;
    public static bool in_transition = false;

    public enum STATE
    {
        START,
        LOOP,
        END
    }

    public override void _Ready()
    {
        if (INSTANCE == null)
            INSTANCE = this;
    }

    public static void StartTransition()
    {
        if (in_transition)
            return;

        if (GetAnimationPlayer() == null)
        {
            Debug.Print("Transition Instance not found");
            return;
        }

        INSTANCE.Start(STATE.START);
    }

    private async void Start(STATE state)
    {
        GetAnimationPlayer().Play("Start_Transition");
        in_transition = true;
        await ToSignal(anim_player, "animation_finished");
        StartLoop();
    }

    private void StartLoop()
    {
        EmitSignal(SignalName.InTransition);
        current_state = STATE.LOOP;
    }

    public async void StopTransition()
    {
        current_state = STATE.END;
        GetAnimationPlayer().Play("Stop_Transition");
        await INSTANCE.ToSignal(INSTANCE.anim_player, "animation_finished");
        in_transition = false;
        INSTANCE.EmitSignal(SignalName.TransitionEndedCompletly);
    }

    public static AnimationPlayer GetAnimationPlayer()
    {
        return INSTANCE.anim_player;
    }

    public static SignalAwaiter TransitionEnded()
    {
        return INSTANCE.ToSignal(INSTANCE, "TransitionEndedCompletly");
    }

    public static SignalAwaiter IsInTransitionLoop()
    {
        return INSTANCE.ToSignal(INSTANCE, "InTransition");
    }
}
