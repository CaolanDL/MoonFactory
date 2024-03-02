using System;
using utils;
using machines;


public enum StructureType
{
    None = 0,
    Crusher,
    Conveyor,
    Merger,
    Splitter,
    Hopper
}

public class Structure : Entity
{
    StructureData data;

    public static Structure CreateStructure(StructureType structureType)
    {
        return StructureFactory.CreateStructure(GlobalData.Instance.structures.Find(data => data.name == structureType.ToString()));
    }

    /*    public static Structure CreateStructure(StructureType structureType)
    {
        switch (structureType)
        { 
            case StructureType.Crusher:
                return new Crusher();

            case StructureType.Conveyor:
                return new Conveyor();

            case StructureType.Merger:
                return new Merger();

            case StructureType.Splitter:
                return new Splitter();

            case StructureType.Hopper:
                return new Hopper();

            default: throw new System.Exception("No structure type enum passed when creating new structure instance");
        } 
    }*/
} 

public class StructureFactory
{ 
    public static Structure CreateStructure(StructureData structureData)
    {
        Type structureType = Type.GetType(structureData.name);

        if(structureType != typeof(Structure)) { throw new Exception("Non-structure Type passed to Factory when creating new structure instance"); }

        else return (Structure)Activator.CreateInstance(structureType); 
    }
}
 
namespace utils
{ 
    public class Conveyor : Machine
    {

    }

    public class Merger : Machine
    {

    }

    public class Splitter : Machine
    {

    }

    public class Hopper : Machine
    {

    }
}

namespace machines
{
    public class Machine : Structure
    {

    }

    public class Crusher : Machine
    {

    } 
} 

