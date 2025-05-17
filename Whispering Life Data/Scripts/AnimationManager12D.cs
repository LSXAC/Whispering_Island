using System;
using System.Diagnostics;
using Godot;

public partial class AnimationManager12D : AnimationManagerBase
{
    public string[,] animation_matrix = new string[4, 3]
    {
        { "LEFT", "CORNER_DOWN_LEFT", "CORNER_UP_LEFT" },
        { "RIGHT", "CORNER_DOWN_RIGHT", "CORNER_UP_RIGHT" },
        { "UP", "CORNER_RIGHT_UP", "CORNER_LEFT_UP" },
        { "DOWN", "CORNER_RIGHT_DOWN", "CORNER_LEFT_DOWN" },
    };

    public enum ANIMATION_DIRECTION
    {
        LEFT,
        CORNER_DOWN_LEFT,
        CORNER_UP_LEFT,
        RIGHT,
        CORNER_DOWN_RIGHT,
        CORNER_UP_RIGHT,
        UP,
        CORNER_RIGHT_UP,
        CORNER_LEFT_UP,
        DOWN,
        CORNER_RIGHT_DOWN,
        CORNER_LEFT_DOWN
    }

    public string GetAnimationNameFromMatrix(ANIMATION_DIRECTION dir)
    {
        int y = (int)dir / 3;
        int x = (int)dir - y * 3;
        Debug.Print(dir.ToString() + " | " + x + " | " + y);
        return animation_matrix[y, x];
    }

    public void SetAnimation(Belt.Direction from_direction, Belt.Direction to_direction)
    {
        if (to_direction == Belt.Direction.Left)
            switch (from_direction)
            {
                case Belt.Direction.Right:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.LEFT);
                    break;
                case Belt.Direction.Top:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_DOWN_LEFT);
                    break;
                case Belt.Direction.Down:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_UP_LEFT);
                    break;
            }
        if (to_direction == Belt.Direction.Right)
            switch (from_direction)
            {
                case Belt.Direction.Left:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.RIGHT);
                    break;
                case Belt.Direction.Top:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_DOWN_RIGHT);
                    break;
                case Belt.Direction.Down:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_UP_RIGHT);
                    break;
            }
        if (to_direction == Belt.Direction.Top)
            switch (from_direction)
            {
                case Belt.Direction.Down:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.UP);
                    break;
                case Belt.Direction.Left:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_RIGHT_UP);
                    break;
                case Belt.Direction.Right:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_LEFT_UP);
                    break;
            }
        if (to_direction == Belt.Direction.Down)
            switch (from_direction)
            {
                case Belt.Direction.Top:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.DOWN);
                    break;
                case Belt.Direction.Left:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_RIGHT_DOWN);
                    break;
                case Belt.Direction.Right:
                    dir = GetAnimationNameFromMatrix(dir: ANIMATION_DIRECTION.CORNER_LEFT_DOWN);
                    break;
            }
        RefTimer ref_timer = GlobalAnimationTimer.INSTANCE.GetCurrentFrame();
        setFrame(ref_timer.frame, GlobalAnimationTimer.INSTANCE.TimeLeft);
    }
}
