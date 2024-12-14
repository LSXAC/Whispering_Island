using System;
using System.Diagnostics;
using Godot;

public partial class TransitionManager : CanvasLayer
{
    public static TransitionManager INSTANCE = null;

    [Signal]
    public delegate void InTransitionEventHandler();

    [Signal]
    public delegate void TransitionEndedCompletlyEventHandler();

    [Export]
    public AnimationPlayer anim_player;

    public static bool in_transition = false;

    public static STATE current_state = STATE.START;

    public enum STATE
    {
        START,
        LOOP,
        END
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (INSTANCE == null)
            INSTANCE = this;
    }

    public static AnimationPlayer GetAnimationPlayer()
    {
        return INSTANCE.anim_player;
    }

    public static void StartTransition()
    {
        if (in_transition)
        {
            Debug.Print("ALREADY IN TRANSITION!");
            return;
        }
        INSTANCE.Start(STATE.START);
    }

    private async void Start(STATE state)
    {
        Debug.Print("Start Transition");
        GetAnimationPlayer().Play("Start_Transition");
        in_transition = true;
        await ToSignal(anim_player, "animation_finished");
        StartLoop();
    }

    private void StartLoop()
    {
        Debug.Print("In Loop Transition");
        EmitSignal(SignalName.InTransition);
        current_state = STATE.LOOP;
        //GetAnimationPlayer().Play("Transition_Loop"); // Looping mag er nicht :(, irgendwie While Loading: "Loop Scene" neustarten immer
    }

    public async void StopTransition()
    {
        Debug.Print("Stop Transition");
        current_state = STATE.END;
        //await INSTANCE.ToSignal(INSTANCE.anim_player, "animation_finished");
        GetAnimationPlayer().Play("Stop_Transition");
        await INSTANCE.ToSignal(INSTANCE.anim_player, "animation_finished");
        in_transition = false;
        Debug.Print("Last");
        INSTANCE.EmitSignal(SignalName.TransitionEndedCompletly);
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
