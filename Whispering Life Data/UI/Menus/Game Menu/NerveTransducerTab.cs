using System;
using Godot;

public partial class NerveTransducerTab : Control
{
    public static NerveTransducerTab instance;
    public NerveTransducer nerve_transducer;

    public override void _Ready()
    {
        instance = this;
    }

    public void SetNerveTransducer(NerveTransducer nerveTransducer)
    {
        this.nerve_transducer = nerveTransducer;
        UpdateUI();
    }

    public void UpdateUI() { }
}
