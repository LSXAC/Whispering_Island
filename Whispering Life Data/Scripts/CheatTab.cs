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
        Game_Manager.INSTANCE.game_timer.Start();
        QuestManager.INSTANCE.StartTimer();
        timeStateLabel.Text = "Time is running! HELP!";
    }

    public void OnPauseTimeButton()
    {
        Game_Manager.INSTANCE.game_timer.Stop();
        QuestManager.INSTANCE.PauseTimer();
        timeStateLabel.Text = "Time is stopped! Relief!";
    }

    private void SetItemsInList()
    {
        item_list.Clear();
        foreach (var (id, item_info) in Inventory.INSTANCE.item_Types)
            item_list.AddItem(
                TranslationServer.Translate(item_info.item_name),
                item_info.texture,
                true
            );
    }

    public void OnDeselectAll()
    {
        item_list.DeselectAll();
    }

    public void OnCreateItems()
    {
        int[] items = item_list.GetSelectedItems();
        foreach (int i in items)
            Inventory.INSTANCE.AddItem(
                Inventory.INSTANCE.item_Types[(InventoryBase.ITEM_ID)i],
                Inventory.INSTANCE.item_Types[(InventoryBase.ITEM_ID)i].max_slot_amount,
                Inventory.INSTANCE.inventory_items
            );
        OnDeselectAll();
    }
}
