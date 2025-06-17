using Godot;

public partial class GlobalAnimationTimer : Timer
{
    public static GlobalAnimationTimer INSTANCE;
    public int current_frame = 0;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public RefTimer GetCurrentFrame()
    {
        return new RefTimer(current_frame);
    }

    public void OnAnimationGlobalTimerTimeout()
    {
        current_frame++;
        if (current_frame > 3)
            current_frame = 0;
    }
}

public class RefTimer
{
    public int frame;

    public RefTimer(int frame)
    {
        this.frame = frame + 1;
        if (this.frame > 3)
            this.frame = 0;
    }
}
