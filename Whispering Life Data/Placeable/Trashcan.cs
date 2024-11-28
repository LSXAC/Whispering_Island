using Godot;
using System;

public partial class Trashcan : Chest
{
	[Export]
	public Timer deletion_timer;

	public void OnDeletionTimerTimeout()
	{
		DeleteItems();
	}

	public void DeleteItems()
	{
		this.chest_items = new ItemSave[20];
	}
}
