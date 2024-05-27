using ExtensionMethods;
using Logistics;
using MoonFactory.Interfaces;
using RoverTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics; 
using UnityEngine;

public abstract class Structure : Entity, IDemolishable
{
    public static Action<Structure> StructureConstructed;
    public event Action OnDemolishedEvent;

    public static List<Structure> Structures = new List<Structure>();
    public StructureData StructureData;
    public DisplayObject DisplayObject = null; 

    public enum PortType { Port, Input, Output }
    public Dictionary<TinyTransform, TinyTransform> WorldSpacePorts = new(); // lhs StructureData unput, output, or port local transform; rhs World space transformation
    public Dictionary<TinyTransform, PortType> WorldSpacePortTypes = new();

    public SupplyPort SupplyPort;
    public RequestPort RequestPort;

    public ManagedTask demolishTask = new();
    public bool flaggedForDemolition;

    private Electrical.Node _electricalNode = null;
    public Electrical.Node ElectricalNode
    {
        get => _electricalNode;
        set
        { 
            _electricalNode = value;
            _electricalNode.Parent = this;
        }
    } 

    bool isInterfaceOpen = false;
    static StaticInterface activeInterface;

    GameObject demolishIcon = null;

    public virtual bool PlayConstructedAnimation { get => true; }

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

        RegisterWorldSpacePorts(); 
        ConnectOuputs();
        AddPorts(); 
        if (_electricalNode != null) { _electricalNode.Constructed(); }

        if (PlayConstructedAnimation)
        {
            var effect = GameObject.Instantiate(RenderData.Instance.ConstructedEffect, DisplayObject.transform.position, Quaternion.identity, DisplayObject.transform);
            effect.transform.localScale = Vector3.one * (size.x * size.y);
            DisplayObject.gameObject.AddComponent<ShakeOnAwake>();
            DisplayObject.PlaySound(AudioData.Instance.World_StructureConstructed, 0.8f);
        } 

        OnConstructed();
        StructureConstructed?.Invoke(this);
    }

    public virtual void OnConstructed() { }

    
    void FlattenTerrain() // From Legacy Terrain System
    {
        var occupyingLocations = GetOccupyingLocations(this);
        var floorGrid = GameManager.Instance.GameWorld.floorGrid;

        //Debug.Log(occupyingLocations.Count);

        foreach (var location in occupyingLocations)
        {
            var floorLocation = floorGrid.GetLocationAt(location.position);

            if (location == null) { continue; } 
            if (floorLocation == null) { continue; } 

            floorLocation.RemoveEntity();
            var newTile = new FloorTile(GameManager.Instance.GameWorld.TerrainGenerator.GenerateTileAt(position));
            floorGrid.AddEntity(newTile, floorLocation.position);
        } 
    } 

    public virtual bool CanDemolish()
    {
        return true;
    }

    public int DemolishTime => StructureData.timeToBuild;

    public void ToggleDemolition()
    {
        if (flaggedForDemolition)
        {
            CancelDemolition();
        }
        else
        {
            FlagForDemolition();
        }
    }

    public void FlagForDemolition()
    {
        var demolishTask = new RoverTasks.DemolishTask(this);

        flaggedForDemolition = true;

        this.demolishTask.TryCreateTask(demolishTask);
    }

    public void CancelDemolition()
    {
        demolishTask.CancelTask();

        flaggedForDemolition = false;
    }

    public void Demolish()
    {
        GameObject.Destroy(demolishIcon); demolishIcon = null;
        Structures.Remove(this); 
        DisconnectInputs(); 
        RemoveEntity(); 
        DeletePorts(); 
        DisplayObject.DemolishAnimation(); 
        OnDemolishedEvent?.Invoke(); 
        OnDemolished();
        if(GameManager.Instance.Lander != null) 
            foreach(var rq in StructureData.requiredResources) 
                GameManager.Instance.Lander?.inventory.TryAddResource(rq); 
    } 

    public virtual void OnDemolished() { }

    public void Tick()
    {
        OnTick();
    }

    public void FrameUpdate()
    {
        if (flaggedForDemolition && demolishIcon == null)
        {
            demolishIcon = GameObject.Instantiate(MenuData.Instance.DemolishSprite, GameManager.Instance.HUDManager.WorldIconContainer);
        }
        if(demolishIcon != null)
        {
            demolishIcon.transform.position = DisplayObject.transform.position.ToScreenPosition();
            demolishIcon.transform.localScale = Vector3.one / GameManager.Instance.CameraController.zoom;
            if (!flaggedForDemolition)
            {
                GameObject.Destroy(demolishIcon);
            }
        }

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

    public void RegisterWorldSpacePorts()
    {
        foreach(var port in StructureData.inputs)
            RegisterPort(port, PortType.Input);
        foreach (var port in StructureData.outputs)
            RegisterPort(port, PortType.Output);
        foreach (var port in StructureData.ports)
            RegisterPort(port, PortType.Port);
        void RegisterPort(TinyTransform port, PortType type)
        {
            var worldSpaceTransform = new TinyTransform(port.position.Rotate(rotation) + position, port.rotation.Rotate(rotation));
            WorldSpacePorts.Add(port, worldSpaceTransform);
            WorldSpacePortTypes.Add(worldSpaceTransform, type); 
        }
    }

    public virtual void AddPorts()
    {

    }

    public void DeletePorts()
    {
        if(SupplyPort != null) { SupplyPort.Delete(); SupplyPort = null; }
        if (RequestPort != null) { RequestPort.Delete(); RequestPort = null; }
    }

    public virtual void ConnectOuputs()
    {
        foreach (var output in StructureData.outputs)
        {
            ConnectAt(output, PortType.Output);
        }
        foreach (var port in StructureData.ports)
        {
            ConnectAt(port, PortType.Port);
        }

        void ConnectAt(TinyTransform tinyTransform, PortType portType)
        {
            var portTransform = WorldSpacePorts[tinyTransform];

            var entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(portTransform.position);

            if (entity == null) return;
             
            if (entity.GetType() == typeof(Conveyor))
            {
                var conveyor = (Conveyor)entity;

                if (conveyor.parentChain.conveyors[0] != conveyor) { return; }

                var offsetRotation = portTransform.rotation;

                if (portType == PortType.Port)
                {
                    offsetRotation = offsetRotation.Rotate(2);
                } 

                if (conveyor.rotation == offsetRotation)
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.Straight); 
                    OnOutputFound();
                }
                else if (conveyor.rotation == offsetRotation.Rotate(1))
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.RightTurn);
                    OnOutputFound();
                }
                else if (conveyor.rotation == offsetRotation.Rotate(-1))
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.LeftTurn);
                    OnOutputFound();
                }
            } 
        }
    }

    public virtual void DisconnectInputs()
    {
        foreach (var output in StructureData.inputs)
        {
            DisconnectAt(output, PortType.Input);
        }
        foreach (var output in StructureData.ports)
        {
            DisconnectAt(output, PortType.Port);
        }

        void DisconnectAt(TinyTransform tinyTransform, PortType portType)
        {
            var portTransform = WorldSpacePorts[tinyTransform];

            var entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(portTransform.position);

            if (entity == null) return;

            if (entity.GetType().IsSubclassOf(typeof(Structure)))
            {
                var structure = (Structure)entity;

                // Extrude the port to match its equivelant input.
                var offsetTransform = new TinyTransform();
                if (portType == PortType.Port)
                {
                    offsetTransform.position = portTransform.position - portTransform.rotation.ToInt2();
                }
                else
                {
                    offsetTransform.position = portTransform.position + portTransform.rotation.ToInt2();
                }

                if (structure.WorldSpacePortTypes.TryGetValue(offsetTransform, out PortType _portType))
                {
                    if (_portType == PortType.Output)
                    {
                        structure.OnOutputLost();
                    }
                }
            }
        }
    }

    public virtual void OnOutputFound()
    {

    }

    public virtual void OnOutputLost()
    {

    }

    public virtual bool TryInputItem(ResourceData resource, TinyTransform inputTransform)
    {
        return false;
    }

    static int2 int2one = new int2(0, 1);

    public bool TryOutputItem(ResourceData resource, TinyTransform localPort)
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;

        var entityAtLocation = worldGrid.GetEntityAt(WorldSpacePorts[localPort].position); 

        if (entityAtLocation == null) return false;

        // Recess the port to match its equivelant input.
        TinyTransform offsetOutputLocation = WorldSpacePorts[localPort];
        if (WorldSpacePortTypes[WorldSpacePorts[localPort]] == PortType.Port) { offsetOutputLocation.rotation += 2; }
        offsetOutputLocation.position = offsetOutputLocation.position - offsetOutputLocation.rotation.ToInt2(); 

        if (entityAtLocation.GetType().IsSubclassOf(typeof(Structure)))
        {
            var structure = (Structure)entityAtLocation;
            if (structure.TryInputItem(resource, offsetOutputLocation)) return true;
        } 
        return false;
    } 



    public virtual void OnTick() { }

    public virtual void OnFrameUpdate() { }

    public virtual void OnClicked(Vector3 mousePosition) { } 

    public void OpenInterfaceOnHUD(GameObject interfacePrefab, Vector3 mousePosition)
    {
        var success = GameManager.Instance.HUDManager.OpenInterface(interfacePrefab, this, mousePosition);

        if (success)
        {
            isInterfaceOpen = true;
            activeInterface = GameManager.Instance.HUDManager.openInterface;
        }

        TryUpdateInterface();
    }

    public virtual void TryUpdateInterface()
    {
        if (isInterfaceOpen)
        {
            activeInterface.UpdateUI();
        }
    }

    public void OnInterfaceClosed()
    {
        isInterfaceOpen = false;
    } 
}

// Reference: http://www.jkfill.com/2010/12/29/self-registering-factories-in-c-sharp/

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