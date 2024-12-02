using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ProcessBuilding : MachineBase
{
    [Export]
    public int fuel_count = 0;

    [Export]
    public int max_fuel_count = 250;

    [Export]
    public int fuel_left = 0;

    [Export]
    public ItemInfo fuel_item_info;

    [Export]
    public Timer crafting_timer;

    [Export]
    public Timer state_timer;

    [Export]
    public Array<Machine_Recipe> recipes = new Array<Machine_Recipe>();

    [Export]
    public int import_count = 0;

    [Export]
    public ItemInfo import_item_info;

    [Export]
    public int export_count = 0;

    [Export]
    public ItemInfo export_item_info;
    public bool is_crafting = false;
    public int ui_progress = 0;
    public int current_recipe = 0;
    public int progress = 0;

    public bool inStartTransition = false;
    public bool inEndTransition = false;

    public void OnCraftingTimerTimeout()
    {
        if (FurnaceTab.INSTANCE.process_building == this)
            FurnaceTab.INSTANCE.UpdateProgressbar(progress);

        if (FurnaceTab.INSTANCE.process_building == this)
            FurnaceTab.INSTANCE.UpdateFuelProgressbar(
                (int)(((double)fuel_left / max_fuel_count) * 100)
            );

        if (progress >= 100)
        {
            export_item_info = recipes[current_recipe].export_item_info;
            export_count += recipes[current_recipe].export_amount;
            is_crafting = false;
            crafting_timer.Stop();
            progress = 0;
            if (FurnaceTab.INSTANCE.process_building == this)
                FurnaceTab.INSTANCE.UpdateProgressbar(progress);
            FurnaceTab.INSTANCE.UpdateFurnaceUI();
            return;
        }
        progress += 5;
        fuel_left -= 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (is_crafting)
            return;

        if (import_item_info == null)
        {
            FurnaceTab.INSTANCE.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC_NO_RESOURCE"
            );
            return;
        }
        if (!SelectAndCheckCanCraft())
        {
            FurnaceTab.INSTANCE.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC_NO_RECIPE"
            );
            return;
        }
        if (import_count <= recipes[current_recipe].import_amount - 1)
            return;

        if (fuel_left < 20)
            if (fuel_count > 0)
            {
                fuel_left += (
                    (BurnableType)
                        fuel_item_info.item_types_arr[
                            fuel_item_info.GetTypeIndex(ItemInfo.Type.BURNABLE)
                        ]
                ).burntime;
                fuel_count--;
            }
            else
            {
                FurnaceTab.INSTANCE.description_Label.Text = TranslationServer.Translate(
                    "FURNACE_MENU_DESC_NO_FUEL"
                );
                return;
            }

        if (!machine_enabled)
        {
            FurnaceTab.INSTANCE.description_Label.Text = "";
            return;
        }
        else
        {
            FurnaceTab.INSTANCE.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC"
            );
        }
        is_crafting = true;
        import_count -= recipes[current_recipe].import_amount;
        if (FurnaceTab.INSTANCE.process_building == this)
            FurnaceTab.INSTANCE.UpdateFurnaceUI();
        crafting_timer.Start();
    }

    private bool SelectAndCheckCanCraft()
    {
        if (import_item_info == null)
            return false;

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].unlockRequirement != null || recipes[i].unlockRequirement.Count > 0)
                if (!GlobalFunctions.CheckAllRequirements(recipes[i].unlockRequirement))
                    continue;

            if (recipes[i].import_item_info == import_item_info)
            {
                current_recipe = i;
                if (export_item_info == null)
                    return true;
                else if (export_item_info == recipes[i].export_item_info)
                    return true;
            }
        }
        return false;
    }

    public void OnMachineTimeOut()
    {
        if (!inEndTransition)
        {
            FurnaceTab.INSTANCE.switch_button.Disabled = true;
            if (ui_progress > 0)
                ui_progress -= 2;

            if (ui_progress <= 0)
            {
                ui_progress = 0;
                if (FurnaceTab.INSTANCE.process_building == this)
                    FurnaceTab.INSTANCE.switch_button.Disabled = false;

                FurnaceTab.INSTANCE.safty_panel.Visible = false;
                FurnaceTab.INSTANCE.ChangeEndStateLabel(true);
                state_timer.Stop();
                inStartTransition = false;
            }
            if (FurnaceTab.INSTANCE.process_building == this)
                FurnaceTab.INSTANCE.SetMachineProgressbar(ui_progress);
            return;
        }

        if (!inStartTransition)
        {
            FurnaceTab.INSTANCE.switch_button.Disabled = true;
            if (ui_progress < 100)
                ui_progress += 2;

            if (ui_progress >= 100)
            {
                ui_progress = 100;
                FurnaceTab.INSTANCE.ChangeEndStateLabel(false);
                if (FurnaceTab.INSTANCE.process_building == this)
                    FurnaceTab.INSTANCE.switch_button.Disabled = false;
                state_timer.Stop();
                inEndTransition = false;
            }
            if (FurnaceTab.INSTANCE.process_building == this)
                FurnaceTab.INSTANCE.SetMachineProgressbar(ui_progress);
        }
    }
}
