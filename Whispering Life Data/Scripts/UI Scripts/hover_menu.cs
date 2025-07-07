using System;
using System.Diagnostics;
using Godot;

public partial class hover_menu : PanelContainer
{
    public static hover_menu instance = null;
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
        instance = this;
        DisableHoverMenu();
    }

    public static void InitHoverMenu(Node2D node)
    {
        instance.current_object = node;
        if (node is Building_Node building_node)
        {
            instance.title_container.Visible = false;
            instance.descriptiion_container.Visible = false;
            instance.resource_type_container.Visible = false;
            instance.resource_type_level_container.Visible = false;
            instance.hitpoint_container.Visible = false;
            instance.object_type_container.Visible = false;

            instance.process_bar_container.Visible = false;
            instance.process_fuel_container.Visible = false;
            instance.process_input_container.Visible = false;
            instance.process_output_container.Visible = false;

            instance.chest_slots_container.Visible = false;

            instance.Line1.Visible = false;
            instance.Line2.Visible = false;

            instance.title_container.Visible = true;
            instance.descriptiion_container.Visible = true;
            instance.object_type_container.Visible = true;

            //Title ---------------
            instance.title_content.Text = TranslationServer.Translate(building_node.GetTitle());

            //Description -------------
            instance.description_content.Text = TranslationServer.Translate(
                building_node.GetDescription()
            );

            //Object Type -------------
            if (node is MineableObject)
                instance.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_RESOURCE"
                );
            if (node is ProcessBuilding)
                instance.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_MACHINE_PROCESSING"
                );
            if (node is ProductionMachine)
                instance.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_MACHINE_PRODUCTION"
                );
            if (node is ChestBase)
                instance.object_type_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_OBJECT_TYPE_CHEST"
                );
        }

        if (node is MineableObject ro)
        {
            instance.hitpoint_container.Visible = true;
            instance.resource_type_container.Visible = true;
            instance.resource_type_level_container.Visible = true;
            instance.Line1.Visible = true;

            //Hitpoints if Ressource
            instance.hitpoint_content.Text = ro.current_durability + "/" + ro.max_durability;

            //Type if Ressource
            instance.resource_type_content.Text = ro.tool_type.ToString();

            //Collect Level if Ressource
            instance.resource_type_level_content.Text = ro.mining_level.ToString();
        }

        if (node is ProcessBuilding pb)
        {
            if (pb == null)
            {
                Debug.Print("PB NULL");
                return;
            }
            instance.process_bar_container.Visible = true;
            instance.process_fuel_container.Visible = true;
            instance.process_input_container.Visible = true;
            instance.process_output_container.Visible = true;
            instance.Line2.Visible = true;

            instance.process_bar_content.Text = pb.progress + "/" + 100;
            instance.process_fuel_content.Text = pb.fuel_left + "x";

            if (pb.item_array[(int)FurnaceTab.SlotType.EXPORT] != null)
                instance.process_output_content.Text =
                    pb.item_array[(int)FurnaceTab.SlotType.EXPORT].amount
                    + "x "
                    + TranslationServer.Translate(
                        Inventory
                            .ITEM_TYPES[
                                (Inventory.ITEM_ID)
                                    pb.item_array[(int)FurnaceTab.SlotType.EXPORT].item_id
                            ]
                            .name
                    );
            else
                instance.process_output_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_PROCESS_NO_ITEM"
                );

            if (pb.item_array[(int)FurnaceTab.SlotType.IMPORT] != null)
                instance.process_input_content.Text =
                    pb.item_array[(int)FurnaceTab.SlotType.IMPORT].amount
                    + "x "
                    + TranslationServer.Translate(
                        Inventory
                            .ITEM_TYPES[
                                (Inventory.ITEM_ID)
                                    pb.item_array[(int)FurnaceTab.SlotType.IMPORT].item_id
                            ]
                            .name
                    );
            else
                instance.process_input_content.Text = TranslationServer.Translate(
                    "HOVER_MENU_PROCESS_NO_ITEM"
                );
        }

        if (node is ProductionMachine production_machine)
        {
            if (production_machine == null)
            {
                Debug.Print("PM NULL");
                return;
            }
            instance.process_bar_container.Visible = true;
            instance.process_output_container.Visible = true;
            instance.Line2.Visible = true;

            instance.process_bar_content.Text = production_machine.progress + "/" + 100;
            instance.process_output_content.Text =
                production_machine.count
                + "x "
                + TranslationServer.Translate(production_machine.output_item_resource.name);
        }

        if (node is ChestBase chest)
        {
            if (chest == null)
            {
                Debug.Print("chest NULL");
                return;
            }
            instance.chest_slots_container.Visible = true;
            instance.Line2.Visible = true;

            instance.chest_slots_content.Text = chest.GetAmountOfFreeSlots() + "/" + 20;
        }

        EnableHoverMenu();
    }

    public static void DisableHoverMenu()
    {
        instance.Visible = false;
        instance.current_object = null;
    }

    public static void EnableHoverMenu()
    {
        instance.Visible = true;
    }
}
