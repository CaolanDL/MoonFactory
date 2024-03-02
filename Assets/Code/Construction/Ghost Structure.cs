

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