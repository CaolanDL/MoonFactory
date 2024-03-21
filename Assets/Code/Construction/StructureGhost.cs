

public class StructureGhost : Entity
{
    public StructureData structureData;

    public StructureGhost(StructureData structureData)
    {
        this.structureData = structureData;
    }

    public void OnPlaced()
    {
        TaskManager.AddTask(new BuildStructure(this));
    }

    public void FinishConstruction()
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        worldGrid.RemoveEntity(position); 
         
        Structure newStructure = StructureFactory.CreateStructure(structureData);

        worldGrid.TryAddEntity(newStructure, position, rotation);

        GameManager.Instance.ConstructionManager.ghosts.Remove(this);

        newStructure.Constructed();
    }
}