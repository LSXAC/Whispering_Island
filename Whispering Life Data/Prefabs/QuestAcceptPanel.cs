using System;
using Godot;

public partial class QuestAcceptPanel : Panel
{
    [Export]
    public Label title_label,
        description_label;

    [Export]
    public Button confirm_button;

    public void InitAcceptPanel() { }

    public void OnButton()
    {
        Visible = false;
    }
}
