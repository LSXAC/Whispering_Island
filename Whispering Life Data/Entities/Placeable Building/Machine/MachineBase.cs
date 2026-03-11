using System;
using Godot;
using Godot.Collections;

public partial class MachineBase : placeable_building
{
    [Export]
    public bool machine_enabled = false;

    [Export]
    public ColorRect machine_activ_color_rect;

    public Color machine_active_color = new Color(0f, 1f, 0f, 1f);
    public Color machine_inactive_color = new Color(1f, 0.0f, 0.0f, 1f);

    public Array<Giver> givers = new Array<Giver>();

    public Array<Taker> takers = new Array<Taker>();

    public Array<PathConnectArea> pathConnectAreas = new Array<PathConnectArea>();

    public Array<GpuParticles2D> particles = new Array<GpuParticles2D>();

    public override void _Ready()
    {
        base._Ready();
        UpdateActivColorRect();
        takers.Clear();
        givers.Clear();

        foreach (Node node in GetChildren())
        {
            if (node is Taker taker)
                takers.Add(taker);
            if (node is Giver giver)
                givers.Add(giver);
            if (node is PathConnectArea pathConnectArea)
                pathConnectAreas.Add(pathConnectArea);
            if (node is GpuParticles2D gp2d)
                particles.Add(gp2d);
        }

        //can cause extrem lags, when multiple machines placed! - Only use, when few Machines
        /*if (givers.Count == 0 || takers.Count == 0)
            Logger.PrintEmptyList(); */
    }

    public void UpdateActivColorRect()
    {
        if (machine_activ_color_rect == null)
        {
            Logger.NodeIsNull(machine_activ_color_rect);
            return;
        }
        if (machine_enabled)
            machine_activ_color_rect.Color = machine_active_color;
        else
            machine_activ_color_rect.Color = machine_inactive_color;
    }

    public void DisableAreas()
    {
        DisableGivers();
        DisableTakers();
        DisableParticles();
    }

    public void EnableAreas()
    {
        EnableGivers();
        EnableTakers();
        EnableParticles();
    }

    public void DisableParticles()
    {
        foreach (GpuParticles2D gp2d in particles)
            gp2d.Visible = false;
    }

    public void DisableTakers()
    {
        foreach (Taker taker in takers)
            taker.DisableMonitorable();
    }

    public void DisableGivers()
    {
        foreach (Giver giver in givers)
            giver.DisableMonitoring();
    }

    public void EnableParticles()
    {
        foreach (GpuParticles2D gp2d in particles)
            gp2d.Visible = true;
    }

    public void EnableTakers()
    {
        foreach (Taker taker in takers)
            taker.EnableMonitorable();
    }

    public void EnableGivers()
    {
        foreach (Giver giver in givers)
            giver.EnableMonitoring();
    }

    public override Resource Save()
    {
        return new MachineSave(building_id, Position, Scale, machine_enabled);
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            Position = machine_save.pos;
            Scale = machine_save.scale;
            machine_enabled = machine_save.machine_enabled;
            UpdateActivColorRect();
        }
        else
            Logger.PrintWrongSaveType();
    }
}
