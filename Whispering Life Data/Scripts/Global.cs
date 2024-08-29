using System;
using System.Diagnostics;
using System.Numerics;
using DialogueManagerRuntime;
using Godot;

public partial class Global : Node2D
{
    public static float GetDistanceToPlayer(Godot.Vector2 nodePosition)
    {
        return Player.INSTANCE.GlobalPosition.DistanceTo(nodePosition);
    }

    public static void MoveCamera(Godot.Vector2 pos)
    {
        Player.camera.Enabled = false;
        Game_Manager.INSTANCE.cutscene_camera.GlobalPosition = Player.camera.GlobalPosition;
        Game_Manager.INSTANCE.cutscene_camera.Enabled = true;
        Game_Manager.INSTANCE.cutscene_camera.Position = pos;
    }

    public static void StartAfterTutorial()
    {
        Game_Manager.tutorial_finished = true;
        Game_Manager.INSTANCE.game_timer.Start();
        QuestManager.INSTANCE.StartQuest();
    }

    public static void InDialogue()
    {
        Game_Manager.In_Cutscene = true;
        Debug.Print("Enter Diualogue");
    }

    public static void QueueFreeTree()
    {
        Tutorial.INSTANCE.Tree.QueueFree();
    }

    public static void LeaveDialogue()
    {
        Debug.Print("Leave");
        Game_Manager.INSTANCE.cutscene_camera.Enabled = false;
        Player.camera.Position = Game_Manager.INSTANCE.cutscene_camera.Position;
        Player.camera.Enabled = true;
        Player.camera.Position = Godot.Vector2.Zero;
        Game_Manager.In_Cutscene = false;
    }

    public static void OutlineTree()
    {
        Tutorial.INSTANCE.Tree.Visible = true;
        Tutorial.INSTANCE.shadow.Visible = true;
        Tutorial.INSTANCE.Tree.Material = Tutorial.INSTANCE.outline_shader;
    }

    public static void RemoveOutlineTree()
    {
        Tutorial.INSTANCE.Tree.Material = null;
        Tutorial.INSTANCE.Tree.Visible = false;
        Tutorial.INSTANCE.shadow.Visible = false;
    }
}
