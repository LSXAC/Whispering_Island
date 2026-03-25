using System;
using Godot;

public partial class NerveTransducerTab : MachinePanel
{
    public static NerveTransducerTab instance;
    public NerveTransducer nerve_transducer;

    public override void _Ready()
    {
        instance = this;
    }

    public void UpdateUI() { }

    public override void SetReference(Node2D nerveTransducer)
    {
        this.nerve_transducer = (NerveTransducer)nerveTransducer;
        UpdateUI();
    }
}
