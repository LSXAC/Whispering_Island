using System;
using System.Collections.Generic;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;

public partial class CutsceneManager : Node2D
{
    private Queue<(Resource resource, string cutscene_name)> cutscene_queue =
        new Queue<(Resource, string)>();

    private Resource current_cutscene_resource;
    private string current_cutscene_name;

    public Camera2D cutscene_camera;

    public static CutsceneManager instance;
    public static bool In_Cutscene = false;
    public static bool skip_cutscenes = false;
    public PackedScene balloon_scene = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://cowvna244n74k")
    );

    [Signal]
    public delegate void CutsceneFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        instance = this;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        DialogueManager.TranslationSource = TranslationSource.CSV;
    }

    public void ClearCutsceneQueue()
    {
        cutscene_queue.Clear();
    }

    public void QueueCutscene(Resource res, string cutscene_name)
    {
        // Wenn Cutscenes übersprungen werden sollen, nicht in Queue einreihen
        if (skip_cutscenes)
            return;

        // Prüfe ob die gleiche Cutscene gerade abgespielt wird
        if (current_cutscene_resource == res && current_cutscene_name == cutscene_name)
        {
            GD.PrintErr(
                $"Cutscene '{cutscene_name}' is currently playing is not added to the queue again."
            );
            return;
        }

        // Prüfe ob die gleiche Cutscene bereits in der Queue ist
        foreach (var (queuedResource, queuedName) in cutscene_queue)
        {
            if (queuedResource == res && queuedName == cutscene_name)
            {
                GD.PrintErr(
                    $"Cutscene '{cutscene_name}' is already in the queue and will not be added again."
                );
                return;
            }
        }

        cutscene_queue.Enqueue((res, cutscene_name));

        if (!In_Cutscene)
            PlayNextCutscene();
    }

    private async void PlayNextCutscene()
    {
        if (cutscene_queue.Count == 0)
        {
            In_Cutscene = false;
            GlobalFunctions.LeaveDialogue();
            return;
        }
        TimeManager.PauseTime();
        GameMenu.instance.OnExitButton();
        BuildMenu.instance.CloseWindow();
        GameMenu.CloseLastWindow();

        In_Cutscene = true;
        var (resource, cutscene_name) = cutscene_queue.Dequeue();

        current_cutscene_resource = resource;
        current_cutscene_name = cutscene_name;

        GlobalFunctions.MoveCameraToPosition(new Vector2(0, -256));
        GlobalFunctions.InDialogue();

        var balloon = DialogueManager.ShowDialogueBalloonScene(
            balloon_scene,
            resource,
            cutscene_name
        );

        await ToSignal(this, SignalName.CutsceneFinished);
        TimeManager.ResumeTime();
        current_cutscene_resource = null;
        current_cutscene_name = "";
        PlayNextCutscene();
    }

    public int GetQueueCount()
    {
        return cutscene_queue.Count;
    }

    public void ClearQueue()
    {
        cutscene_queue.Clear();
        In_Cutscene = false;
    }
}
