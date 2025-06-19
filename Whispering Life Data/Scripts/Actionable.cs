using System;
using DialogueManagerRuntime;
using Godot;

public partial class Actionable : Area2D
{
    [Export]
    public Resource dialogueResource;

    [Export]
    public string diaglog_start = "";

    public void Action()
    {
        if (diaglog_start.Equals(""))
            return;
        if (dialogueResource == null)
            return;
        if (!GameManager.In_Cutscene)
        {
            GlobalFunctions.InDialogue();
            DialogueManager.ShowDialogueBalloon(dialogueResource, diaglog_start);
        }
    }
}
