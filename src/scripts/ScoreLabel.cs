using Godot;
using System;

public class ScoreLabel : Label
{
    private void OnPlayerAdvanced(int score)
    {
        SetText(score.ToString());
    }
}