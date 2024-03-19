
using Unity.Mathematics;

public class Task
{
    public bool isComplete = false;
}

public class BuildStructure : Task
{
    public int2 structureLocation;
    public StructureData structureData;
}