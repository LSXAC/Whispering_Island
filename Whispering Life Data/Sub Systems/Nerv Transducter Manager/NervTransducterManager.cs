using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class NervTransducterManager : Node2D
{
    public List<NerveTransducer> nerv_transducers = new List<NerveTransducer>();
    public static NervTransducterManager instance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instance = this;
        var timer = new Timer();
        timer.WaitTime = 1.0f; // Sekunden
        timer.OneShot = false; // Wiederholt sich
        timer.Autostart = true;

        AddChild(timer);
        timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout()
    {
        Debug.Print("Nerv Reduction: " + instance.GetNervReduction());
    }

    public float GetNervReduction()
    {
        float amount = 0;
        foreach (NerveTransducer transducer in nerv_transducers)
            amount += transducer.GetNervReductionAmount();
        return amount;
    }

    public void AddNervTransducer(NerveTransducer nerve_transducer)
    {
        if (nerv_transducers.Contains(nerve_transducer))
            return;

        nerv_transducers.Add(nerve_transducer);
    }

    public void RemoveNervTransducer(NerveTransducer nerve_transducer)
    {
        if (nerv_transducers.Contains(nerve_transducer))
            nerv_transducers.Remove(nerve_transducer);
    }
}
