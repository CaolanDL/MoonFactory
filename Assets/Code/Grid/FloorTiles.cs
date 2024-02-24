using UnityEngine;

public class FloorTile : Entity
{
    public FloorTileData tileData;

    public FloorTile(FloorTileData tileData)
    {
        this.tileData = tileData;
    }
}