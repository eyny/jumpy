using Godot;
using System;

public class GameScene : Node2D
{
    public GameScene()
    {
        GD.Randomize();
    }

    public void OnPlayerDied(int score)
    {
        GetTree().ReloadCurrentScene();
    }
}
