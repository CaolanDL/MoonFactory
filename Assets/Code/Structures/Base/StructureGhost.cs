

public class StructureGhost : Entity
{
    public StructureData data;

    public StructureGhost(StructureData structureData)
    {
        data = structureData;
    }

    public void FinishConstruction()
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        worldGrid.RemoveEntity(position); 
         
        Structure newStructure = StructureFactory.CreateStructure(data);

        worldGrid.AddEntity(newStructure, position, rotation);

        GameManager.Instance.ConstructionManager.ghosts.Remove(this);
    }
}