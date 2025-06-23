using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ProcessBuilding : MachineBase
{
    [Export]
    public ItemSave[] item_array = new ItemSave[3] { null, null, null };

    [Export]
    public int max_fuel_count = 250;

    [Export]
    public int fuel_left = 0;

    [Export]
    public Timer crafting_timer;

    [Export]
    public Timer state_timer;

    [Export]
    public Array<Machine_Recipe> recipes = new Array<Machine_Recipe>();

    public bool is_crafting = false;
    public int ui_progress = 0;
    public int current_recipe = 0;
    public int progress = 0;

    public bool inStartTransition = false;
    public bool inEndTransition = false;

    public ItemInfo GetItemResource(FurnaceTab.SlotType slotType)
    {
        if (item_array[(int)slotType] == null)
            return null;

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_array[(int)slotType].item_id];
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        FurnaceTab.instance.SetProcessBuilding(this);
        GameMenu.instance.OnOpenFurnaceTab();
    }

    public void OnCraftingTimerTimeout()
    {
        if (FurnaceTab.instance.process_building == this)
            FurnaceTab.instance.UpdateProgressbar(progress);

        if (FurnaceTab.instance.process_building == this)
            FurnaceTab.instance.UpdateFuelProgressbar(
                (int)((double)fuel_left / max_fuel_count * 100)
            );

        if (progress >= 100)
        {
            if (item_array[(int)FurnaceTab.SlotType.EXPORT] != null)
                item_array[(int)FurnaceTab.SlotType.EXPORT].amount += recipes[
                    current_recipe
                ].amount_in_export;
            else
                item_array[(int)FurnaceTab.SlotType.EXPORT] = new ItemSave(
                    (int)recipes[current_recipe].export_item_info.id,
                    recipes[current_recipe].amount_in_export
                );

            is_crafting = false;
            crafting_timer.Stop();
            progress = 0;
            if (FurnaceTab.instance.process_building == this)
                FurnaceTab.instance.UpdateProgressbar(progress);
            if (FurnaceTab.instance.process_building == this)
                FurnaceTab.instance.UpdateFurnaceUI();
            return;
        }

        progress += 5;
        fuel_left -= 1;

        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (is_crafting)
            return;

        if (item_array[(int)FurnaceTab.SlotType.IMPORT] == null)
        {
            FurnaceTab.instance.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC_NO_RESOURCE"
            );
            return;
        }

        if (!SelectAndCheckCanCraft())
        {
            FurnaceTab.instance.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC_NO_RECIPE"
            );
            return;
        }
        if (
            item_array[(int)FurnaceTab.SlotType.IMPORT].amount
            <= recipes[current_recipe].amount_in_import - 1
        )
            return;

        if (fuel_left < 20)
            if (item_array[(int)FurnaceTab.SlotType.FUEL] != null)
            {
                if (item_array[(int)FurnaceTab.SlotType.FUEL].amount > 0)
                {
                    ItemInfo info = Inventory.ITEM_TYPES[
                        (Inventory.ITEM_ID)item_array[(int)FurnaceTab.SlotType.FUEL].item_id
                    ];
                    fuel_left += (
                        (BurnableAttribute)
                            info.attributes[info.GetAttributeIndex(ItemAttribute.TYPE.BURNABLE)]
                    ).burntime;
                    item_array[(int)FurnaceTab.SlotType.FUEL].amount--;
                }
                else
                {
                    FurnaceTab.instance.description_Label.Text = TranslationServer.Translate(
                        "FURNACE_MENU_DESC_NO_FUEL"
                    );
                    return;
                }
            }
            else
            {
                FurnaceTab.instance.description_Label.Text = TranslationServer.Translate(
                    "FURNACE_MENU_DESC_NO_FUEL"
                );
                return;
            }

        if (!machine_enabled)
        {
            FurnaceTab.instance.description_Label.Text = "";
            return;
        }
        else
        {
            FurnaceTab.instance.description_Label.Text = TranslationServer.Translate(
                "FURNACE_MENU_DESC"
            );
        }

        is_crafting = true;
        item_array[(int)FurnaceTab.SlotType.IMPORT].amount -= recipes[
            current_recipe
        ].amount_in_import;
        if (FurnaceTab.instance.process_building == this)
            FurnaceTab.instance.UpdateFurnaceUI();
        crafting_timer.Start();
    }

    private bool SelectAndCheckCanCraft()
    {
        if (item_array[(int)FurnaceTab.SlotType.IMPORT] == null)
            return false;

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].unlockRequirement != null || recipes[i].unlockRequirement.Count > 0)
                if (!GlobalFunctions.CheckResearchRequirements(recipes[i].unlockRequirement))
                    continue;

            if (item_array[(int)FurnaceTab.SlotType.IMPORT] != null)
                if (
                    recipes[i].import_item_info
                    == Inventory.ITEM_TYPES[
                        (Inventory.ITEM_ID)item_array[(int)FurnaceTab.SlotType.IMPORT].item_id
                    ]
                )
                {
                    current_recipe = i;
                    if (item_array[(int)FurnaceTab.SlotType.EXPORT] == null)
                        return true;
                    else if (
                        recipes[i].export_item_info
                        == Inventory.ITEM_TYPES[
                            (Inventory.ITEM_ID)item_array[(int)FurnaceTab.SlotType.EXPORT].item_id
                        ]
                    )
                        return true;
                }
        }
        return false;
    }

    public void OnMachineTimeOut()
    {
        if (!inEndTransition)
        {
            FurnaceTab.instance.switch_button.Disabled = true;
            if (ui_progress > 0)
                ui_progress -= 2;

            if (ui_progress <= 0)
            {
                ui_progress = 0;
                if (FurnaceTab.instance.process_building == this)
                    FurnaceTab.instance.switch_button.Disabled = false;

                FurnaceTab.instance.safty_panel.Visible = false;
                FurnaceTab.instance.ChangeEndStateLabel(true);
                state_timer.Stop();
                inStartTransition = false;
            }
            if (FurnaceTab.instance.process_building == this)
                FurnaceTab.instance.SetMachineProgressbar(ui_progress);
            return;
        }

        if (!inStartTransition)
        {
            FurnaceTab.instance.switch_button.Disabled = true;
            if (ui_progress < 100)
                ui_progress += 2;

            if (ui_progress >= 100)
            {
                ui_progress = 100;
                FurnaceTab.instance.ChangeEndStateLabel(false);
                if (FurnaceTab.instance.process_building == this)
                    FurnaceTab.instance.switch_button.Disabled = false;
                state_timer.Stop();
                inEndTransition = false;
            }
            if (FurnaceTab.instance.process_building == this)
                FurnaceTab.instance.SetMachineProgressbar(ui_progress);
        }
    }
}
