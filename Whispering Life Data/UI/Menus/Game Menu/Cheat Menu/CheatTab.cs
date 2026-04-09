using System;
using Godot;
using Godot.Collections;

public partial class CheatTab : ColorRect
{
    [Export]
    public ItemList item_list;

    [Export]
    public CheckBox is_poisoned_box;
    private Array<ItemInfo> item_infos = new Array<ItemInfo>();

    [Export]
    public Label timeStateLabel;

    public Label cutscene_skip_label;

    public void OnVisiblityChange()
    {
        SetItemsInList();
    }

    public void SetQuestTimeTo(int time)
    {
        QuestManager.current_quest_time = time;
    }

    public void AddMood(float amount)
    {
        MonsterIsland.instance.SetMood(MonsterIsland.instance.GetMood() + amount);
    }

    public void OnCheckBoxToggled(bool check_box)
    {
        GameManager.dev_build_mode = check_box;
    }

    public void OnResumeTimeButton()
    {
        TimeManager.ResumeTime();
        timeStateLabel.Text = "Time is running! HELP!";
    }

    public void OnPauseTimeButton()
    {
        TimeManager.PauseTime();
        timeStateLabel.Text = "Time is stopped! Relief!";
    }

    public void OnTimeMultiplier(int mult)
    {
        GameManager.time_multiplier = mult;
        timeStateLabel.Text = $"Time multiplier set to {mult / 5}x";
    }

    private void SetItemsInList()
    {
        item_list.Clear();
        foreach (var (id, info) in Inventory.ITEM_TYPES)
        {
            item_infos.Add(info);
            item_list.AddItem(TranslationServer.Translate(info.name), info.texture, true);
        }
    }

    public void OnDeselectAll()
    {
        item_list.DeselectAll();
    }

    public void OnCreateItems()
    {
        int[] items = item_list.GetSelectedItems();
        foreach (int i in items)
        {
            try
            {
                if (is_poisoned_box.ButtonPressed)
                    PlayerInventoryUI.instance.AddItem(
                        new Item(
                            item_infos[i],
                            item_infos[i].max_stackable_size,
                            state: Item.STATE.POISONED
                        ),
                        PlayerInventoryUI.instance.inventory_items
                    );
                else
                    PlayerInventoryUI.instance.AddItem(
                        new Item(
                            item_infos[i],
                            item_infos[i].max_stackable_size,
                            state: Item.STATE.NORMAL
                        ),
                        PlayerInventoryUI.instance.inventory_items
                    );
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error creating item with ID {i}: {e.Message} {item_infos[i].name}");
            }
        }
        OnDeselectAll();
    }

    public void OnToggleCutsceneSkip(bool skip_cutscenes)
    {
        CutsceneManager.skip_cutscenes = skip_cutscenes;
        if (cutscene_skip_label != null)
        {
            cutscene_skip_label.Text = skip_cutscenes ? "Cutscenes: SKIPPED" : "Cutscenes: PLAYING";
        }
    }

    public void OnApplyQuestPenalty()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.ApplyPenality();
        }
        else
        {
            GD.PrintErr("QuestManager instance not found!");
        }
    }
}
