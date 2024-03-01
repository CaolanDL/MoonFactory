
using UnityEngine;

public enum StructureType
{
    None = 0,
    Crusher
}


public class Structure : Entity
{
    StructureData data;

    public static Structure CreateStructure(StructureType structureType)
    {
        if(structureType == StructureType.Crusher)
        {
            return new Crusher();
        }

        throw new System.Exception("No structure type enum passed when creating new structure instance"); 
    }
}

public class GhostStructure : Entity
{
    public StructureData data;

    public GhostStructure(StructureData structureData)
    {
        data = structureData;
    }

    public void FinishConstruction()
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var thisLocation = worldGrid.GetLocationAt(position);

        thisLocation.entity = null; //remove this entity from the worldgrid

        GameManager.Instance.constructionManager.ghosts.Remove(this);

        Structure newStructure = Structure.CreateStructure(data.structureType);

        worldGrid.AddEntity(newStructure, position);
    }
}

public class Machine : Structure
{

}

public class Crusher : Machine
{

}