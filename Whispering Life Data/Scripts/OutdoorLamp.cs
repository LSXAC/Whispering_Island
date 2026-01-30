using System;
using Godot;

public partial class OutdoorLamp : placeable_building
{
    [Export]
    public LightBehaviour lightBehaviour;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        lightBehaviour.Enabled = !lightBehaviour.Enabled;
    }

    public override Resource Save()
    {
        return new PlaceableSave(building_id, Position);
    }

    public override void Load(Resource save)
    {
        LoadPlaceable(save);
    }
}
