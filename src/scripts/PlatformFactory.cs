using Godot;
using System;

public class PlatformFactory : Node
{
    [Signal] delegate void OnAdvanced(float distance);

    [Export] public float maxSideSpeed = 120f;
    [Export] public float maxSideSpeedInc = 6f;
    [Export] public float minSideSpeedCoef = 0.4f;
    [Export] public float topPlatformBotY = -50f;
    [Export] public double midPlatformTopY = 200f;
    [Export] public double midPlatformBotY = 500f;
    [Export] public float botPlatformY = 840f;
    [Export] public int actLevels = 10;

    private PackedScene platformScene;
    private float midPlatformY;

    public override void _Ready()
    {
        platformScene = (PackedScene)ResourceLoader.Load("res://scenes/platform.tscn");
        SetInitPlatform();
    }

    private void SetInitPlatform()
    {
        Platform initPlatform = (Platform)GetOwner().GetNode("initPlatform");
        midPlatformY = GetRandomMidY();
        initPlatform.SetPosition(new Vector2(0, midPlatformY));
        initPlatform.SetAndPlaySideAnim(GenerateSideSpeed());
    }

    public void OnPlayerAdvanced(int score)
    {
        Platform newPlatform = CreatePlatform(score);
        SetPosAndDest(newPlatform);
        newPlatform.SetAndPlaySideAnim(GenerateSideSpeed());
    }

    private void SetPosAndDest(Platform platform)
    {
        float distance = botPlatformY - midPlatformY;
        float topPlatDestY = GetRandomMidY();
        float newPosY = topPlatDestY - distance;
        if (newPosY > topPlatformBotY)
            newPosY = topPlatformBotY;
        platform.SetPosition(new Vector2(0, newPosY));
        float midPlatDestY = botPlatformY;
        float botPlatDestY = botPlatformY + distance;
        GetTree().CallGroup("platforms", "AnimateDown", topPlatDestY, midPlatDestY, botPlatDestY);
        midPlatformY = topPlatDestY;
        EmitSignal("OnAdvanced", distance);
    }

    private Platform CreatePlatform(int score)
    {
        Platform platform = (Platform)platformScene.Instance();
        GenerateTiles(platform, score);
        GetParent().AddChild(platform);
        platform.AddToGroup("platforms");
        return platform;
    }

    private void GenerateTiles(Platform platform, int score)
    {
        if (score < actLevels)
            platform.SetTiles("grass", 5);
        else if (score < actLevels * 2)
            platform.SetTiles("dirt", 4);
        else if (score < actLevels * 3)
            platform.SetTiles("mushroom", 3);
        else if (score < actLevels * 4)
            platform.SetTiles("snow", 2);
        else
            platform.SetTiles("castle", 1);
    }

    private void SetPlatformPos(Platform platform)
    {
        float distance = botPlatformY - midPlatformY;
        float topPlatDestY = GetRandomMidY();
        float newPosY = topPlatDestY - distance;
        if (newPosY > topPlatformBotY)
            newPosY = topPlatformBotY;
        platform.SetPosition(new Vector2(0, newPosY));
        float midPlatDestY = botPlatformY;
        float botPlatDestY = botPlatformY + distance;
    }

    private float GetRandomMidY()
    {
        return (float)GD.RandRange(midPlatformTopY, midPlatformBotY);
    }

    private float GenerateSideSpeed()
    {
        float minSideSpeed = maxSideSpeed * minSideSpeedCoef;
        float sideSpeed = (float)GD.RandRange(minSideSpeed, maxSideSpeed);
        maxSideSpeed += maxSideSpeedInc;
        return sideSpeed;
    }
}
