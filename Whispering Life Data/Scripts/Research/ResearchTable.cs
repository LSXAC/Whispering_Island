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

    public override Resource Save()
    {
        return new PlaceableSave(building_id, Position);
    }

    public override void Load(Resource save)
    {
        if (save is PlaceableSave placeable_save)
        {
            Position = placeable_save.pos;
            building_id = placeable_save.building_id;
        }
        else
            Logger.PrintWrongSaveType();
    }
}
