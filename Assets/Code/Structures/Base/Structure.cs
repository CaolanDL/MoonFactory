using System; 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Structure : Entity
{
    public static List<Structure> structures = new List<Structure>();

    public StructureData data;


    public static Structure GetStructure(int2 position)
    {
        foreach (var structure in structures)
        {
            if(structure == null) continue;
            if( structure.position.Equals(position)) return structure;
        }

        return null;
    }
     
} 

public class StructureFactory
{ 
    public static Structure CreateStructure(StructureData structureData)
    {
        Type structureType = Type.GetType(structureData.name);

        if(structureType.IsSubclassOf(typeof(Structure)) != true) { throw new Exception("Non-structure Type passed to Factory when creating new structure instance"); }

        Structure newStructure = (Structure)Activator.CreateInstance(structureType);

        newStructure.data = structureData;

        Structure.structures.Add(newStructure); 

        return newStructure; 
    }
} 