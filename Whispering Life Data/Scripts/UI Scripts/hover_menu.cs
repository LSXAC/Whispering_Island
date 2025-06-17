using System;
using System.Diagnostics;
using Godot;

public partial class hover_menu : PanelContainer
{
    public static hover_menu INSTANCE = null;
    public Node2D current_object = null;

    [Export]
    public HBoxContainer title_container,
        descriptiion_container,
        object_type_container,
        hitpoint_container,
        resource_type_container,
        resource_type_level_container,
        process_bar_container,
        process_fuel_container,
        process_input_container,
        process_output_container,
        chest_slots_container;

    [Export]
    public Label title_content,
        description_content;

    [Export]
    public Label object_type_content;

    [Export]
    public Label hitpoint_content;

    [Export]
    public Label resource_type_content;

    [Export]
    public Label resource_type_level_content;

    [Export]
    public Label process_bar_content,
        process_fuel_content,
        process_input_content,
        process_output_content;

    [Export]
    public Label chest_slots_content;

    [Export]
    public ColorRect Line1,
        Line2;

    public override void _Ready()
    {
        INSTANCE = this;
        DisableHoverMenu();
    }

    public static void InitHoverMenu(Node2D node)
    {
        INSTANCE.current_object = node;
        if (node is Building_Node b)
        {
            INSTANCE.title_container.Visible = false;
            INSTANCE.descriptiion_container.Visible = false;
            INSTANCE.resource_type_container.Visible = false;
            INSTANCE.resource_type_level_container.Visible = false;
            INSTANCE.hitpoint_container.Visible = false;
            INSTANCE.object_type_container.Visible = false;

            INSTANCE.process_bar_container.Visible = false;
            INSTANCE.process_fuel_container.Visible = false;
            INSTANCE.process_input_container.Visible = false;
            INSTANCE.process_output_container.Visible = false;

            INSTANCE.chest_slots_container.Visible = false;

            INSTANCE.Line1.Visible = false;
            INSTANCE.Line2.Visible = false;

            if (b.GetSprite() == null)
                return;

            INSTANCE.title_container.Visible = true;
            INSTANCE.descriptiion_container.Visible = true;
            INSTANCE.object_type_container.Visible = true;

            //Title ---------------
            INSTANCE.title_content.Text = TranslationServer.Translate(b.GetTitle());

            //Description -------------
            INSTANCE.description_content.Text = TranslationServer.Translate(b.GetDescription());

            //Object Type -------------
            if (node is MineableObject)
                INSTANCE.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_RESOURCE"
                );
            if (node is ProcessBuilding)
                INSTANCE.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_MACHINE_PROCESSING"
                );
            if (node is ProductionMachine)
                INSTANCE.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_MACHINE_PRODUCTION"
                );
            if (node is ChestBase)
                INSTANCE.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_CHEST"
                );
        }

        if (node is MineableObject ro)
        {
            INSTANCE.hitpoint_container.Visible = true;
            INSTANCE.resource_type_container.Visible = true;
            INSTANCE.resource_type_level_container.Visible = true;
            INSTANCE.Line1.Visible = true;

            //Hitpoints if Ressource
            INSTANCE.hitpoint_content.Text = ro.current_durability + "/" + ro.max_durability;

            //Type if Ressource
            INSTANCE.resource_type_content.Text = ro.type.ToString();

            //Collect Level if Ressource
            INSTANCE.resource_type_level_content.Text = ro.type_level.ToString();
        }

        if (node is ProcessBuilding pb)
        {
            if (pb == null)
            {
                Debug.Print("PB NULL");
                return;
            }
            INSTANCE.process_bar_container.Visible = true;
            INSTANCE.process_fuel_container.Visible = true;
            INSTANCE.process_input_container.Visible = true;
            INSTANCE.process_output_container.Visible = true;
            INSTANCE.Line2.Visible = true;

            INSTANCE.process_bar_content.Text = pb.progress + "/" + 100;
            INSTANCE.process_fuel_content.Text = pb.fuel_left + "x";

            if (pb.item_array[(int)FurnaceTab.SlotType.EXPORT] != null)
                INSTANCE.process_output_content.Text =
                    pb.item_array[(int)FurnaceTab.SlotType.EXPORT].amount
                    + "x "
                    + TranslationServer.Translate(
                        Inventory
                            .INSTANCE
                            .item_Types[
                                (InventoryBase.ITEM_ID)
                                    pb.item_array[(int)FurnaceTab.SlotType.EXPORT].item_id
                            ]
                            .item_name
                    );
            else
                INSTANCE.process_output_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_PROCESS_NO_ITEM"
                );

            if (pb.item_array[(int)FurnaceTab.SlotType.IMPORT] != null)
                INSTANCE.process_input_content.Text =
                    pb.item_array[(int)FurnaceTab.SlotType.IMPORT].amount
                    + "x "
                    + TranslationServer.Translate(
                        Inventory
                            .INSTANCE
                            .item_Types[
                                (InventoryBase.ITEM_ID)
                                    pb.item_array[(int)FurnaceTab.SlotType.IMPORT].item_id
                            ]
                            .item_name
                    );
            else
                INSTANCE.process_input_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_PROCESS_NO_ITEM"
                );
        }

        if (node is ProductionMachine pm)
        {
            if (pm == null)
            {
                Debug.Print("PM NULL");
                return;
            }
            INSTANCE.process_bar_container.Visible = true;
            INSTANCE.process_output_container.Visible = true;
            INSTANCE.Line2.Visible = true;

            INSTANCE.process_bar_content.Text = pm.progress + "/" + 100;
            INSTANCE.process_output_content.Text =
                pm.production_count
                + "x "
                + TranslationServer.Translate(pm.production_item_info.item_name);
        }

        if (node is ChestBase chest)
        {
            if (chest == null)
            {
                Debug.Print("chest NULL");
                return;
            }
            INSTANCE.chest_slots_container.Visible = true;
            INSTANCE.Line2.Visible = true;

            INSTANCE.chest_slots_content.Text = chest.GetAmountOfFreeSlots() + "/" + 20;
        }

        EnableHoverMenu();
    }

    public static void DisableHoverMenu()
    {
        INSTANCE.Visible = false;
        INSTANCE.current_object = null;
    }

    public static void EnableHoverMenu()
    {
        INSTANCE.Visible = true;
    }
}
