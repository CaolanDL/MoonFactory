using ExtensionMethods;
using RoverTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

public abstract class Structure : Entity
{
    public static Action<Structure> StructureConstructed;
    public event Action OnDemolishedEvent;

    public static List<Structure> Structures = new List<Structure>(); 
    public StructureData StructureData; 
    public DisplayObject DisplayObject = null;

    public ManagedTask demolishTask = new();
    public bool flaggedForDemolition; 

    private Electrical.Node _electricalNode = null;
    public Electrical.Node ElectricalNode
    {
        get => _electricalNode;
        set 
        {   // Bind a new Electrical Node and initialise it;
            //? Might be code smell; Maybe Dangerous; Should work but consider migration to Init method or node constructor, passing structure.
            _electricalNode = value;
            _electricalNode.Parent = this;
            OnDemolishedEvent += _electricalNode.Demolished; 
        }
    }


    public void Initialise()
    {
        base.size = new DataStructs.byte2(StructureData.size.x, StructureData.size.y);

        Structures.Add(this);

        OnInitialise(); 
    }

    public virtual void OnInitialise()
    {

    }

    public static Structure GetStructure(int2 position)
    {
        return Structures.Where(structure => structure != null).FirstOrDefault(structure => structure.position.Equals(position));
    }

    public static void TickAllStructures()
    {
        foreach (var structure in Structures)
        {
            structure.Tick();
        }
    }

    public static void FrameUpdateAllStructures()
    {
        foreach (var structure in Structures)
        {
            structure.FrameUpdate();
        }
    }



    public void Constructed()
    {
        GameObject newDisplayGameObject = UnityEngine.Object.Instantiate(StructureData.displayObject, position.ToVector3(), rotation.ToQuaternion(), GameManager.Instance.transform);

        DisplayObject = newDisplayGameObject.GetComponent<DisplayObject>();

        ConnectOuputs();

        OnConstructed();

        StructureConstructed?.Invoke(this);

        if (_electricalNode != null) { _electricalNode.Constructed(); } 
    }

    public void Demolish()
    {
        Structures.Remove(this);

        RemoveEntity();

        DisplayObject.DemolishAnimation();

        OnDemolishedEvent?.Invoke();

        OnDemolished();
    }

    public void Tick()
    {
        OnTick();
    }

    public void FrameUpdate()
    {
        OnFrameUpdate();
    }

    public void Clicked()
    {
        Clicked(Vector3.zero);
    }

    public void Clicked(Vector3 mousePosition)
    {
        OnClicked(mousePosition);
    }

    public virtual void ConnectOuputs() { }

    public virtual void OnConstructed() { }

    public virtual void OnDemolished() { }

    public virtual void OnTick() { }

    public virtual void OnFrameUpdate() { }

    public virtual void OnClicked(Vector3 mousePosition) { }



    public void FlagForDemolition()
    {
        var demolishTask = new RoverTasks.DemolishStructureTask(this);

        flaggedForDemolition = true;

        this.demolishTask.TryCreateTask(demolishTask);
    }

    public void CancelDemolition()
    {
        demolishTask.CancelTask();

        flaggedForDemolition = false;
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
                Type structureType = derivedStructure.GetType();

                _registry.Add(structureType.Name, structureType);
            }
        }

        return _registry;
    }

    public static Structure CreateStructure(StructureData structureData)
    {
        Structure newStructure = (Structure)Activator.CreateInstance(sTypeRegistry[structureData.name]);

        newStructure.StructureData = structureData;

        newStructure.Initialise(); 

        return newStructure;
    }
}