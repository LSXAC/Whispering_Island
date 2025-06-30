using System;
using Godot;
using Godot.Collections;

public partial class CheatTab : ColorRect
{
    [Export]
    public ItemList item_list;
    private Array<ItemInfo> item_infos = new Array<ItemInfo>();

    [Export]
    public Label timeStateLabel;

    public void OnVisiblityChange()
    {
        SetItemsInList();
    }

    public void SetQuestTimeTo(int time)
    {
        QuestManager.current_quest_time = time;
    }

    public void OnResumeTimeButton()
    {
        TimeManager.instance.game_timer.Start();
        QuestManager.instance.StartTimer();
        timeStateLabel.Text = "Time is running! HELP!";
    }

    public void OnPauseTimeButton()
    {
        TimeManager.instance.game_timer.Stop();
        QuestManager.instance.PauseTimer();
        timeStateLabel.Text = "Time is stopped! Relief!";
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
                PlayerInventoryUI.instance.AddItem(
                    new Item(item_infos[i], item_infos[i].max_stackable_size),
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
}
