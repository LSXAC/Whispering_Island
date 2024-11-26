using System;
using Godot;

public partial class CheatTab : ColorRect
{
    [Export]
    public ItemList item_list;

    [Export]
    public Label timeStateLabel;

    public override void _Ready() { }

    public void OnVisiblityChange()
    {
        SetItemsInList();
    }

    public void OnResumeTimeButton()
    {
        Game_Manager.INSTANCE.game_timer.Start();
        QuestManager.INSTANCE.StartTimer();
        timeStateLabel.Text = "Time is running! HELP";
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
                Inventory.INSTANCE.item_Types[((Inventory.ITEM_ID)i)],
                50,
                Inventory.INSTANCE.inventory_items
            );
        OnDeselectAll();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
