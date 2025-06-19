using System;
using Godot;

public partial class ResearchTable : placeable_building
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.instance.OnOpenResearchTab();
    }
}
