using System; 
using DataStructs;
using ExtensionMethods;
using Unity.Mathematics;

[Serializable]
public class Entity // 12 bytes
{
    public TinyTransform transform; // 9 bytes

    public byte gridId; // 1 byte

    public int2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    } 
     
    public sbyte rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    public byte2 size = new(1, 1); // 2 bytes

    public byte2 centre
    {
        get { return new byte2(size.x / 2, size.y / 2); }
    }  
     
    public Entity()
    {
         
    }

    public Entity GetNeighbor(sbyte rotationFactor)
    { 
        return GameManager.Instance.gameWorld.worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
    } 

    public (int2 xRange, int2 yRange) GetOccupyRegion()
    {
        var rSize = new int2(size.x, size.y).Rotate(rotation);

        var xRange = new int2(position.x, position.x + rSize.x);
        var yRange = new int2(position.y, position.y + rSize.y);

        return (xRange, yRange);
    }
}

public class FloorTile : Entity // Size: 9 bytes
{
    public FloorTileData data; //8 bytes

    public FloorTile(FloorTileData tileData)
    {
        this.data = tileData;
    } 
}