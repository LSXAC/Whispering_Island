using System;
using Godot;

public partial class NerveTransducer : MachineBase
{
    [Export]
    public float nerv_reduction_amount = 0.05f;

    public override void OnMouseClick()
    {
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
            NervTransducterManager.instance.RemoveNervTransducer(this);

        base.OnMouseClick();
        if (!CheckClickDependencies(this))
            return;

        NerveTransducerTab.instance.SetReference(this);
        GameMenu.instance.OnOpenNerveTranducerTab();
    }

    public float GetNervReductionAmount()
    {
        if (has_enough_magic_power)
            return nerv_reduction_amount;
        else
            return 0f;
    }
}
