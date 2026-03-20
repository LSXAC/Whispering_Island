using System;
using Godot;

public partial class HealthBar : Control
{
    [Export]
    public ProgressBar progressBar;

    public int current_health = 100;

    [Export]
    public int max_health = 100;

    public override void _Ready()
    {
        UpdateBar();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void UpdateBar()
    {
        progressBar.Value = current_health;
        progressBar.MaxValue = max_health;
    }

    public void AddHealth(int amount)
    {
        current_health += amount;
        UpdateBar();
    }

    public void RemoveHealth(int amount)
    {
        if (current_health - amount >= 0)
            current_health -= amount;
        UpdateBar();
    }
}
