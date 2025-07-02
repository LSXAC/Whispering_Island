using System;
using System.Diagnostics;
using Godot;

public partial class DayNightManager : CanvasModulate
{
    public static DayNightManager instance;

    [Export]
    public GradientTexture1D dayNightGradient;

    public override void _Ready()
    {
        if (instance == null)
            instance = this;

        // Initial color update
        UpdateColor();
    }

    public void UpdateColor()
    {
        float time = TimeManager.instance.current_game_time;
        // time von 0–1440 (Minuten) auf 0–2*PI (Tagesverlauf) umrechnen
        float normalizedTime = (time / 1440.0f) * Mathf.Pi * 2.0f;
        float value = (Mathf.Sin(normalizedTime - Mathf.Pi / 2.0f) + 1.0f) / 2.0f;

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
