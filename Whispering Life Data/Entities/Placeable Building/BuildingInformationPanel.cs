using System;
using Godot;

public partial class BuildingInformationPanel : Control
{
    [Export]
    public Control no_energy_panel,
        no_recipe_panel,
        no_input_panel;

    public enum PanelType
    {
        NO_ENERGY,
        NO_RECIPE,
        NO_INPUT,
        NO_MATERIALS = NO_INPUT // Alias for clarity: represents no input materials
    }

    public void ActivatePanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.NO_ENERGY:
                no_energy_panel.Visible = true;
                break;
            case PanelType.NO_RECIPE:
                no_recipe_panel.Visible = true;
                break;
            case PanelType.NO_INPUT:
                no_input_panel.Visible = true;
                break;
        }
    }

    public void DeactivatePanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.NO_ENERGY:
                no_energy_panel.Visible = false;
                break;
            case PanelType.NO_RECIPE:
                no_recipe_panel.Visible = false;
                break;
            case PanelType.NO_INPUT:
                no_input_panel.Visible = false;
                break;
        }
    }
}
