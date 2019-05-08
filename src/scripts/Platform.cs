using Godot;
using System;

public class Platform : Node2D
{
    [Export] public int index = 0;
    public float platformAreaLine;
    public float playerLine;

    private TileMap tileMap;
    private AnimationPlayer downAnimPlayer;
    private AnimationPlayer sideAnimPlayer;

    public override void _Ready()
    {
        tileMap = (TileMap)GetNode("tileMap");
        downAnimPlayer = (AnimationPlayer)this.GetNode("downAnimPlayer");
        sideAnimPlayer = (AnimationPlayer)GetNode("tileMap/sideAnimPlayer");
        AddToGroup("platforms");
    }

    public void SetTiles(string tileType, int tileSize = 1)
    {
        int tileID;
        tileMap = (TileMap)GetNode("tileMap");
        string tileMapName = tileMap.GetName();
        if (tileSize == 1)
        {
            tileID = tileMap.GetTileset().FindTileByName(tileType + "Alone");
            tileMap.SetCell(0, 0, tileID);
        }
        else if (tileSize > 1)
        {
            tileID = tileMap.GetTileset().FindTileByName(tileType + "Left");
            tileMap.SetCell(0, 0, tileID);

            tileID = tileMap.GetTileset().FindTileByName(tileType + "Mid");
            for (int i = 1; i < tileSize - 1; i++)
                tileMap.SetCell(i, 0, tileID);

            tileID = tileMap.GetTileset().FindTileByName(tileType + "Right");
            tileMap.SetCell(tileSize - 1, 0, tileID);
        }
    }

    // Turns animation into a unique animation and returns that
    private Animation UniqueAnim(AnimationPlayer animPlayer, string animName)
    {
        Animation uniqueAnim = (Animation)animPlayer.GetAnimation(animName).Duplicate();
        animPlayer.RemoveAnimation(animName);
        animPlayer.AddAnimation(animName, uniqueAnim);
        return uniqueAnim;
    }

    public void AnimateDown(float topPlatDestY, float midPlatDestY, float botPlatDestY)
    {
        Animation downAnim = UniqueAnim(downAnimPlayer, "downAnim");
        Vector2 currPos = this.GetPosition();
        downAnim.TrackSetKeyValue(0, 0, currPos);

        float destPosY = 2000;
        switch (index)
        {
            case 0:
                destPosY = topPlatDestY;
                break;
            case 1:
                destPosY = midPlatDestY;
                break;
            case 2:
                destPosY = botPlatDestY;
                tileMap.SetCollisionLayer(0);
                break;
            case 3:
                QueueFree();
                break;
        }
        index++;

        downAnim.TrackSetKeyValue(0, 1, new Vector2(currPos.x, destPosY));
        downAnimPlayer.Play("downAnim");
    }

    public void SetAndPlaySideAnim(float sideSpeed)
    {
        Vector2 platformSize = tileMap.GetGlobalScale() * tileMap.GetCellSize() * tileMap.GetUsedRect().Size;

        float sideDistance = (int)ProjectSettings.GetSetting("display/window/size/width") - platformSize.x;
        float sideTime = sideDistance / sideSpeed;

        Animation sideAnim = UniqueAnim(sideAnimPlayer, "sideAnim");
        Vector2 currPos = tileMap.GetPosition();
        sideAnim.TrackSetKeyValue(0, 0, currPos);
        sideAnim.TrackInsertKey(0, sideTime, new Vector2(sideDistance, currPos.y));
        sideAnim.SetLength(sideTime * 2);

        float sideStartTime = (float)GD.RandRange(0, sideTime * 2);
        sideAnimPlayer.Play("sideAnim");
        sideAnimPlayer.Advance(sideStartTime);
    }
}