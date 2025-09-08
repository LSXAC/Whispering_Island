using Godot;

public partial class PlayerStamina : Node2D
{
    public static float current_stamina = 1f;
    private float max_stamina = 1f;
    private float stamina_regeneration = 0.0025f;
    public static bool stamina_is_regenerating = false;
    private float speed_mult = 65f;
    private float stamina_use = 0.0025f;

    public void UpdateStaminaDependencies(Vector2 velo)
    {
        if (
            Input.IsActionPressed("Shift")
            && current_stamina > 0f
            && !stamina_is_regenerating
            && (velo.X != 0 || velo.Y != 0)
        )
        {
            Player.instance.Velocity = velo.Normalized() * speed_mult * 1.75f;
            Player.instance.anim.SpeedScale = 1.5f;
            current_stamina -=
                stamina_use
                * 1f
                / (
                    Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_REDUCTION)
                );
            ;
            Player.instance.player_stats.AddFatigue(0.01f);
        }
        else
        {
            if (current_stamina <= 0f)
                stamina_is_regenerating = true;
            if (
                current_stamina
                    >= max_stamina
                        * (
                            Skilltree.instance.GetBonusOfCategory(
                                SkillData.TYPE_CATEGORY.STAMINA_MAX
                            )
                        )
                && stamina_is_regenerating
            )
                stamina_is_regenerating = false;

            Player.instance.player_stats.AddFatigue(0.0025f);
            Player.instance.Velocity = velo * speed_mult;
            Player.instance.anim.SpeedScale = 1f;
        }
    }

    public void RegenerateStamina(Vector2 velo)
    {
        if (
            !Input.IsActionPressed("Shift")
            || stamina_is_regenerating
            || (Input.IsActionPressed("Shift") && velo.X == 0 && velo.Y == 0)
        )
            if (
                current_stamina
                <= max_stamina
                    * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_MAX)
            )
                current_stamina += stamina_regeneration;
    }
}
