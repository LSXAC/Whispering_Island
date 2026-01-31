using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class collect_item_label : Label
{
	int times = 0;
	int max_times = 100;
	public void OnTimerFinished()
	{
		if(times <= max_times) {
			Position += new Vector2(0,-1);
			times++;
		} else {
			this.QueueFree();
		}
	}
}
