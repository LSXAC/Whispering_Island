using System;
using System.Security.Cryptography;
using Godot;

public partial class QuestAcceptPanel : Panel
{
    [Export]
    public Label title_label,
        description_label;

    [Export]
    public Button confirm_button;

    private string[] titles =
    {
        "QUEST_ACCEPT_PANEL_HALF_TIME_TITLE",
        "QUEST_ACCEPT_PANEL_DOUBLE_AMOUNT_TITLE",
        "QUEST_ACCEPT_PANEL_REMOVE_ISLANDS_TITLE"
    };
    private string[] descriptions =
    {
        "QUEST_ACCEPT_PANEL_HALF_TIME_DESC",
        "QUEST_ACCEPT_PANEL_DOUBLE_AMOUNT_DESC",
        "QUEST_ACCEPT_PANEL_REMOVE_ISLANDS_DESC"
    };

    public enum PENEALTY
    {
        HALF_TIME,
        DOUBLE_AMOUNT,
        ISLAND_REMOVE
    }

    public int penealty_id = -1;

    public void InitAcceptPanel(PENEALTY penealty)
    {
        title_label.Text = TranslationServer.Translate(titles[(int)penealty]);
        description_label.Text = TranslationServer.Translate(descriptions[(int)penealty]);
        penealty_id = (int)penealty;
    }

    public void OnButton()
    {
        Visible = false;
    }
}
