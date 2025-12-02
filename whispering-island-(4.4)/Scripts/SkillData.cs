using System;
using Godot;

[GlobalClass]
public partial class SkillData : Resource
{
    [Export]
    public ID id;

    [Export]
    public TYPE_CATEGORY type_category;

    [Export]
    public bool is_big_skill = false;

    [Export]
    public string DisplayName = "";

    [Export]
    public string Description = "";

    [Export]
    public Texture2D Icon;

    [Export]
    public float skill_amount = 0;

    [Export]
    public int required_skill_points = 1;

    public enum ID
    {
        MINING_AMOUNT_1,
        MINING_AMOUNT_2,
        MINING_AMOUNT_3,
        MINING_BONUS_1,
        MINING_BONUS_2,
        DEEP_VEIN,
        TOOL_DURABILITY_1,
        TOOL_DURABILITY_2,
        TOOL_DURABILITY_3,
        TOOL_REGENERATION,
        RESEARCH_SPEED_1,
        RESEARCH_SPEED_2,
        RESEARCH_SPEED_3,
        RESEARCH_GAIN_1,
        RESEARCH_GAIN_2,
        RESEARCH_GAIN_3,
        RESEARCH_GAIN_4,
        INSTANT_THEORIE,
        RESEARCH_COST_1,
        RESEARCH_COST_2,
        RESEARCH_COST_3,
        RECYCLE_UNUSED_DATA,
        ISLAND_CAP_1,
        ISLAND_CAP_2,
        ISLAND_CAP_3,
        ISLAND_CAP_4,
        RESOURCE_REDUCER,
        STAMINA_MAX_1,
        STAMINA_MAX_2,
        STAMINA_MAX_3,
        STAMINA_REDUCTION_1,
        STAMINA_REDUCTION_2,
        STAMINA_REDUCTION_3,
        SECOND_WIND,
    }

    public enum TYPE_CATEGORY
    {
        MINING_AMOUNT,
        MINING_BONUS,
        DEEP_VEIN,
        TOOL_DURABILITY,
        TOOL_REGENERATION,
        RESEARCH_SPEED,
        RESEARCH_GAIN,
        RESEARCH_COST,
        RECYCLE_UNUSED_DATA,
        ISLAND_CAP,
        RESOURCE_REDUCER,
        STAMINA_MAX,
        STAMINA_REDUCTION,
        SECOND_WIND,
        INSTANT_THEORIE
    }
}
