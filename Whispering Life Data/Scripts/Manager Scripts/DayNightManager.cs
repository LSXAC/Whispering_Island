using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class DayNightManager : Node2D
{
    [Export]
    public GradientTexture1D dayNightGradient;

    [Export]
    public Array<CanvasModulate> canvases;

    public void UpdateColor()
    {
        float time = GetParent<TimeManager>().current_game_time;
        // time von 0–1440 (Minuten) auf 0–2*PI (Tagesverlauf) umrechnen
        float normalizedTime = (time / 1440.0f) * Mathf.Pi * 2.0f;
        float value = (Mathf.Sin(normalizedTime - Mathf.Pi / 2.0f) + 1.0f) / 2.0f;

        PlayerUI.instance.time_stripe.SetPointer((time / 1440.0f));
        if (dayNightGradient != null && dayNightGradient.Gradient != null)
        {
            Color color = dayNightGradient.Gradient.Sample(value);
            foreach (CanvasModulate cm in canvases)
                cm.Color = color;
        }
        else
        {
            foreach (CanvasModulate cm in canvases)
                cm.Color = new Color(1, 1, 1, value); // Fallback: nur Alpha ändern
        }
    }
}
