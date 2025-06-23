using System;
using Godot;

public partial class CheatTab : ColorRect
{
    [Export]
    public ItemList item_list;

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
        GameManager.instance.game_timer.Start();
        QuestManager.instance.StartTimer();
        timeStateLabel.Text = "Time is running! HELP!";
    }

    public void OnPauseTimeButton()
    {
        GameManager.instance.game_timer.Stop();
        QuestManager.instance.PauseTimer();
        timeStateLabel.Text = "Time is stopped! Relief!";
    }

    private void SetItemsInList()
    {
        item_list.Clear();
        foreach (var (id, info) in Inventory.ITEM_TYPES)
            item_list.AddItem(TranslationServer.Translate(info.name), info.texture, true);
    }

    public void OnDeselectAll()
    {
        item_list.DeselectAll();
    }

    public void OnCreateItems()
    {
        int[] items = item_list.GetSelectedItems();
        foreach (int i in items)
            PlayerInventoryUI.instance.AddItem(
                new Item(
                    Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i],
                    Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i].max_slot_amount
                ),
                PlayerInventoryUI.instance.inventory_items
            );
        OnDeselectAll();
    }
}
