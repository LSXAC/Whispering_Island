using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public partial class DayNightManager : CanvasModulate
{
    [Export]
    public GradientTexture1D dayNightGradient;

    public void UpdateColor()
    {
        float time = GetParent<TimeManager>().current_game_time;
        // time von 0–1440 (Minuten) auf 0–2*PI (Tagesverlauf) umrechnen
        float normalizedTime = (time / 1440.0f) * Mathf.Pi * 2.0f;
        float value = (Mathf.Sin(normalizedTime - Mathf.Pi / 2.0f) + 1.0f) / 2.0f;

        PlayerUI.instance.time_stripe.SetPointer((time / 1440.0f));
        if (dayNightGradient != null && dayNightGradient.Gradient != null)
        {
            Debug.Print("Color Sample");
            Color color = dayNightGradient.Gradient.Sample(value);
            Color = color;
        }
        else
        {
            Color = new Color(1, 1, 1, value); // Fallback: nur Alpha ändern
        }
    }
}
