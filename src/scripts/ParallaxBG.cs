using Godot;
using System;

public class ParallaxBG : ParallaxBackground
{
    private AnimationPlayer downAnimPlayer;
    private Animation downAnim;

    public override void _Ready()
    {
        downAnimPlayer = (AnimationPlayer)GetNode("downAnimPlayer");
        downAnim = downAnimPlayer.GetAnimation("downAnim");
    }

    public void OnAdvanced(float distance)
    {
        Vector2 currOffset = GetScrollOffset();
        downAnim.TrackSetKeyValue(0, 0, currOffset);
        downAnim.TrackSetKeyValue(0, 1, currOffset + new Vector2(0, distance));
        downAnimPlayer.Play("downAnim");
    }
}
