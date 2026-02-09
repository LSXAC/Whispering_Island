using System;
using Godot;

public partial class QuestTimer : Timer
{
    public void PauseTimer()
    {
        Stop();
    }

    public void StartTimer()
    {
        Start();
        WaitTime = 1;
    }
}
