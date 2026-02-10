using System;
using System.Collections.Generic;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;

public partial class CutsceneManager : Node2D
{
    private Queue<(Resource resource, string cutscene_name)> cutscene_queue =
        new Queue<(Resource, string)>();

    public Camera2D cutscene_camera;

    public static CutsceneManager instance;
    public static bool In_Cutscene = false;

    public override void _Ready()
    {
        base._Ready();
        instance = this;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        DialogueManager.TranslationSource = TranslationSource.CSV;
    }

    public void QueueCutscene(Resource res, string cutscene_name)
    {
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

        In_Cutscene = true;
        var (resource, cutscene_name) = cutscene_queue.Dequeue();

        GlobalFunctions.MoveCameraToPosition(new Vector2(0, -256));
        GlobalFunctions.InDialogue();

        var balloon = DialogueManager.ShowExampleDialogueBalloon(resource, cutscene_name);

        // Warte bis die Cutscene zu Ende ist
        await ToSignal(balloon, "tree_exited");

        // Spiele die nächste Cutscene ab
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
