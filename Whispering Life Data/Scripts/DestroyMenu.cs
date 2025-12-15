using System;
using Godot;
using Godot.Collections;

public partial class DestroyMenu : Control
{
    public static DestroyMenu instance;

    [Export]
    public string translation_string_title = "",
        translation_string_description = "";

    [Export]
    public RichTextLabel quest_description_label,
        quest_title_label;

    [Export]
    public Control parent;

    [Export]
    public Button destroy_button;

    [Export]
    public Array<Item> required_items;
    public static RemoveSign current_sign = null;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnVisiblityChanged()
    {
        if (current_sign == null)
            return;

        required_items = GlobalFunctions.GetNormalListOrDevList(current_sign.required_items);
        quest_title_label.Text = "[center]" + TranslationServer.Translate(translation_string_title);
        quest_description_label.Text =
            "[center]" + TranslationServer.Translate(translation_string_description);
        destroy_button.Disabled = true;

        foreach (Control child in parent.GetChildren())
            child.QueueFree();

        QuestMenu.CreateLabels(required_items, parent, 1);
        if (CheckRequirements())
            destroy_button.Disabled = false;
        else
            destroy_button.Disabled = true;
    }

    public void OnDestroyButton()
    {
        if (current_sign == null)
            return;

        foreach (Item item in required_items)
            PlayerInventoryUI.instance.RemoveItem(item, PlayerInventoryUI.instance.inventory_items);

        RemovableObjectsManager manager = current_sign.removable_objects_Manager;
        manager.removed_objects.Add(current_sign.id);
        current_sign.RemoveShadows();
        current_sign.object_connected.QueueFree();
        current_sign.sprite_anim_manager.shadowNode.RemoveShadow();
        current_sign.QueueFree();
        OnExitButton();
    }

    public void OnExitButton()
    {
        GameMenu.instance.OnCloseDestroyTab();
        current_sign = null;
    }

    public bool CheckRequirements()
    {
        foreach (Item item in required_items)
        {
            Array<Item> iii = PlayerInventoryUI.instance.GetItemFromListOrNull(
                PlayerInventoryUI.instance.GetListOfItemsInInventory(),
                item
            );

            if (iii == null)
                return false;

            int amount = 0;
            foreach (Item i_x in iii)
                amount += i_x.amount;

            if (amount < item.amount)
                return false;
        }
        return true;
    }
}
