using System;
using Godot;
using Godot.Collections;

public partial class HelpTab : ColorRect
{
    [Export]
    public Array<PanelContainer> pages;

    private int current_activ_page = 0;

    public void OnVisiblityChange()
    {
        foreach (PanelContainer pc in pages)
            pc.Visible = false;

        pages[0].Visible = true;
        current_activ_page = 0;
    }

    public void OnPageButton(int index)
    {
        pages[current_activ_page].Visible = false;
        current_activ_page = index;
        pages[index].Visible = true;
    }
}
