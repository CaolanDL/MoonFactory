using ExtensionMethods;
using System; 
using System.Collections.Generic;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine; 

public abstract class Structure : Entity
{
    public static List<Structure> structures = new List<Structure>(); 

    public StructureData structureData;

    private string name { get; } 

    public DisplayObject displayObject = null; 

    public void Initialise ()
    {
        OnInitialise();
    } 

    public virtual void OnInitialise()
    {

    }

    public static Structure GetStructure(int2 position)
    {
        foreach (var structure in structures)
        {
            if(structure == null) continue;
            if( structure.position.Equals(position)) return structure;
        }

        return null;
    }  

    public static void TickAllStructures()
    {
        foreach (var structure in structures)
        {
            structure.Tick();
        }
    }

    public void Constructed()
    {
        GameObject newDisplayGameObject = UnityEngine.Object.Instantiate(structureData.displayObject, position.ToVector3(), rotation.ToQuaternion(), GameManager.Instance.transform);

        displayObject = newDisplayGameObject.GetComponent<DisplayObject>();

        OnConstructed();
    }

    public virtual void OnConstructed()
    {

    }

    public void Tick()
    {
        OnTick();
    }

    public virtual void OnTick()
    {

    }
}

// http://www.jkfill.com/2010/12/29/self-registering-factories-in-c-sharp/

public static class StructureFactory
{ 
    public static Dictionary<string, Type> sTypeRegistry = BuildRegistry();

    private static Dictionary<string, Type> BuildRegistry()
    {
        Dictionary<string, Type> _registry = new();

        Assembly currAssembly = Assembly.GetExecutingAssembly();

        Type baseType = typeof(Structure);

        foreach (Type type in currAssembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(baseType))
            {
                continue;
            }

            Structure derivedStructure = Activator.CreateInstance(type) as Structure;

            if (derivedStructure != null)
            {
                Type structureType =  derivedStructure.GetType();

                _registry.Add(structureType.Name, structureType);
            }
        } 

        return _registry;
    }

    public static Structure CreateStructure(StructureData structureData)
    {
        Structure newStructure = (Structure)Activator.CreateInstance(sTypeRegistry[structureData.name]);
        
        newStructure.structureData = structureData;

        newStructure.Initialise();

        Structure.structures.Add(newStructure);

        return newStructure;
    }

/*
    public static Structure CreateStructure(StructureData structureData)
    {
        Type structureType = Type.GetType(structureData.name);

        if(structureType.IsSubclassOf(typeof(Structure)) != true) { throw new Exception("Non-structure Type passed to Factory when creating new structure instance"); }

        Structure newStructure = (Structure)Activator.CreateInstance(structureType);

        newStructure.data = structureData;

        Structure.structures.Add(newStructure); 

        return newStructure; 
    }*/
} 