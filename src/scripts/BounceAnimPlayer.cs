using Godot;
using System;

public class BounceAnimPlayer : AnimationPlayer
{
    [Export] public float bounceCoef = 0.5f;
    [Export] public float landingCoef = 0.3f;

    private Sprite sprite;
    private Vector2 defaultOffset;

    public override void _Ready()
    {
        sprite = (Sprite)GetParent();
        defaultOffset = sprite.Offset;
    }

    public void OnPlayerPressing()
    {
        GetAnimation("pressing").TrackSetKeyValue(0, 0, sprite.GetScale());
        Play("pressing");
    }

    public void OnPlayerJumped()
    {
        SetSpriteOffset(Vector2.Zero);
        AnimateJumped();
    }

    public void OnPlayerLanded(float landingSpeed)
    {
        SetSpriteOffset(defaultOffset);
        AnimateLanded(landingSpeed);
    }

    private void AnimateJumped()
    {
        //2nd and 3rd keyframe calculation for bouncing animation
        Vector2 pressedScale = sprite.GetScale();
        Vector2 bounceVect = (Vector2.One - pressedScale) * bounceCoef;
        Vector2 scale2 = Vector2.One + bounceVect;
        Vector2 scale3 = Vector2.One - bounceVect * bounceCoef;

        //Set scale property of the first 3 keyframes
        GetAnimation("jumped").TrackSetKeyValue(0, 0, pressedScale);
        GetAnimation("jumped").TrackSetKeyValue(0, 1, scale2);
        GetAnimation("jumped").TrackSetKeyValue(0, 2, scale3);
        Play("jumped");
    }

    private void AnimateLanded(float landingSpeed)
    {
        Vector2 limitScale = (Vector2)GetAnimation("pressing").TrackGetKeyValue(0, 1);
        float shrinkLimitY = 1 - limitScale.y;
        float changeRatioXtoY = (limitScale.x - 1) / shrinkLimitY;

        float shrinkY = (landingSpeed / 2048) * landingCoef;
        if (shrinkY > shrinkLimitY)
            shrinkY = shrinkLimitY;
        float widenX = changeRatioXtoY * shrinkY;

        //2nd, 3rd and 4th keyframe calculation for bouncing animation
        Vector2 scale2 = new Vector2(1 + widenX, 1 - shrinkY);
        Vector2 bounceVect = (scale2 - Vector2.One) * bounceCoef;
        Vector2 scale3 = Vector2.One - bounceVect;

        //Set scale property of the middle 3 keyframes
        GetAnimation("landed").TrackSetKeyValue(0, 1, scale2);
        GetAnimation("landed").TrackSetKeyValue(0, 2, scale3);
        Play("landed");
    }

    // Set the offset value of the sprite without any position change
    private void SetSpriteOffset(Vector2 newOffset)
    {
        float newPosY = sprite.Position.y + (sprite.Offset.y - newOffset.y);
        Vector2 newPosition = new Vector2(sprite.Position.x, newPosY);
        sprite.SetPosition(newPosition);
        sprite.SetOffset(newOffset);
    }
}
