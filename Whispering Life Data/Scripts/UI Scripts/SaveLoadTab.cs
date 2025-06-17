using System;
using Godot;

public partial class SaveLoadTab : ColorRect
{
    [Export]
    public Label lastsave_label;

    [Export]
    public Label fromsave_label;

    public static DateTime dateTime_from_save;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        dateTime_from_save = DateTime.Now;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;
        UpdateText();
    }

    public void OnVisiblityChange()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        fromsave_label.Text =
            TranslationServer.Translate("SAVELOAD_MENU_SAVE_FROM")
            + ": "
            + dateTime_from_save.ToString();

        TimeSpan sub = DateTime.Now.Subtract(dateTime_from_save);
        if (sub.TotalDays >= 1)
        {
            lastsave_label.Text =
                TranslationServer.Translate("SAVELOAD_MENU_SAVE_LAST") + ": ~" + sub.Days + " Days";
            return;
        }
        if (sub.TotalHours >= 1)
        {
            lastsave_label.Text =
                TranslationServer.Translate("SAVELOAD_MENU_SAVE_LAST") + ": ~" + sub.Hours + " h";
            return;
        }
        if (sub.TotalMinutes >= 1)
        {
            lastsave_label.Text =
                TranslationServer.Translate("SAVELOAD_MENU_SAVE_LAST")
                + ": ~"
                + sub.Minutes
                + " min";
            return;
        }
        lastsave_label.Text =
            TranslationServer.Translate("SAVELOAD_MENU_SAVE_LAST") + ": ~" + sub.Seconds + " s";
        return;
    }
}
