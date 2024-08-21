using Godot;
public partial class Player_Stamina : Node2D {

	public static float current_stamina = 1f;
	private float max_stamina = 1f;
	private float stamina_regeneration = 0.0025f;
	public static bool stamina_is_regenerating = false;
    private float speed_mult = 75f;
	private float stamina_use = 0.005f;

    public void UpdateStaminaDependencies(float velo_x, float velo_y)
    {
        if(Input.IsActionPressed("Shift") && current_stamina > 0f && !stamina_is_regenerating && (velo_x != 0 || velo_y != 0)) {
			Player.instance.Velocity = new Vector2(velo_x,velo_y).Normalized() * speed_mult * 1.75f;
			Player.instance.anim.SpeedScale = 1.35f * 1.5f;
			current_stamina -= stamina_use;
		}
		else {
			if(current_stamina <= 0f)
				stamina_is_regenerating = true;
			if(current_stamina >= max_stamina && stamina_is_regenerating)
				stamina_is_regenerating = false;

			Player.instance.Velocity = new Vector2(velo_x,velo_y) * speed_mult;
			Player.instance.anim.SpeedScale = 1.35f;
		}
		
		if(!Input.IsActionPressed("Shift") || stamina_is_regenerating || (Input.IsActionPressed("Shift") && velo_x == 0 && velo_y == 0))
			if(current_stamina <= max_stamina)
				current_stamina += stamina_regeneration;
    }
}