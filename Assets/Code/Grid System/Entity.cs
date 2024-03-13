using System; 
using DataStructs;
using ExtensionMethods;
using Unity.Mathematics;

[Serializable]
public class Entity
{
    public TinyTransform transform;

    public byte gridId;

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

    public byte2 size = new(1, 1);

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
}

public class FloorTile : Entity // Size: 9 bytes
{
    public FloorTileData data; //8 bytes

    public FloorTile(FloorTileData tileData)
    {
        this.data = tileData;
    } 
}