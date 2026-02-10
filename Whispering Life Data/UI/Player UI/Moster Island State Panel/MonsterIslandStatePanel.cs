using System;
using Godot;
using Godot.Collections;

public partial class MonsterIslandStatePanel : Panel
{
    [Export]
    public Label state_label,
        stability_label;

    [Export]
    public TextureRect stability_icon,
        state_icon;

    [Export]
    public Array<Texture2D> state_icons = new Array<Texture2D>();

    [Export]
    public Array<Texture2D> stability_icons = new Array<Texture2D>();

    public void UpdateMoodItem(MonsterIslandStateManager.STATE state)
    {
        state_label.Text = TranslationServer.Translate(state.ToString());
        state_icon.Texture = state_icons[(int)state];
    }

    public void UpdateStabiltyItem(float stability)
    {
        int stability_index = Mathf.RoundToInt(stability * (stability_icons.Count - 1));
        stability_label.Text = $"Stability: {stability:P0}";
        stability_icon.Texture = stability_icons[stability_index];
    }
}
