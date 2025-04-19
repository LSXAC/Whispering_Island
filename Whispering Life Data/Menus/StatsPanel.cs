using System;
using Godot;

public partial class StatsPanel : ColorRect
{
	[Export]
	public VBoxContainer stats_container;

	public enum stat_types
	{
		ATTACK,
		DEFENSE,
		FORESTRY,
		MINING,
		FARMING
	};
}
