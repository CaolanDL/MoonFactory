 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods;
using DataStructs;
using static UnityEngine.EventSystems.EventTrigger;
using Terrain;

public class ConstructionManager
{
    public StructureData GhostStructureData;

    public List<StructureGhost> Ghosts = new List<StructureGhost>(); 

    //[SerializeField] public Material GhostMaterial;

    [SerializeField] private sbyte _ghostRotation = 0;
    [SerializeField] public sbyte GhostRotation
    {
        get { return _ghostRotation; }
        set { _ghostRotation = (sbyte)((value % 4 + 4) % 4); }
    } 

    public void StartPlacingGhosts(StructureData structureData)
    {
        _ghostRotation = 0;
        GhostStructureData = structureData;
    }  

    public void PlaceGhost(double2 mousePosition)
    {
        var ghostGridPosition = GetMouseGhostPosition(); 

        if (IsGhostBlocked(ghostGridPosition)) { return; } 
        if(!TutorialProxy.IsPlacementCorrect(ghostGridPosition, GhostRotation)) { PlayErrorSound(); return; }

        var worldGrid = GameManager.Instance.GameWorld.worldGrid; 
        StructureGhost newGhostStructure = new(GhostStructureData);

        if (worldGrid.TryAddEntity(newGhostStructure, ghostGridPosition, _ghostRotation) != null)
        {
            Ghosts.Add(newGhostStructure);
            newGhostStructure.OnPlaced();

            if(DevFlags.InstantBuilding)
            {
                newGhostStructure.FinishConstruction();
            } 
        }

        if(GhostStructureData.name == "StaticDrill") TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillPlaced); 
    }

    void PlayErrorSound() => AudioManager.Instance.PlaySound(AudioData.Instance.UI_Error); 

    public void RotateGhost(sbyte direction)
    {
        GhostRotation += direction;
    } 

    public int2 GetMouseGhostPosition()
    {
        var inputManager = GameManager.Instance.PlayerInputManager;

        if (GhostStructureData.size.x > 1 && GhostStructureData.size.y > 1)
        { 
            var n = (-GhostStructureData.centre).Rotate(GhostRotation * 90);

            int2 nI = n.ToInt2() + inputManager.MouseGridPositon;

            return nI;
        }
        else
        {
            return inputManager.MouseGridPositon;
        }
    }

    public (int2 xRange, int2 yRange) GetOccupyRegion(int2 position)
    {
        var rSize = new int2(GhostStructureData.size.x, GhostStructureData.size.y).Rotate(GhostRotation);
        var xRange = new int2(position.x, position.x + rSize.x);
        var yRange = new int2(position.y, position.y + rSize.y);

        return (xRange, yRange);
    } 

    /// <summary>
    /// Force a structure to spawn at a location. Development use only.
    /// </summary> 
    public Structure ForceSpawnStructure(int2 position, sbyte rotation, StructureData structureData)
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;

        worldGrid.RemoveEntity(position);

        Structure newStructure = StructureFactory.CreateStructure(structureData);

        worldGrid.TryAddEntity(newStructure, position, rotation);

        newStructure.Constructed();

        return newStructure;
    } 

    public void DrawGhosts()
    { 
        if (Ghosts.Count != 0)
        { 
            foreach (StructureGhost ghost in Ghosts)
            { 
                ghost.OnTick();

                var matrix = MatrixConstruction.CreateTransformMatrix(ghost.position, ghost.rotation);

                Graphics.DrawMesh(ghost.structureData.ghostMesh, matrix, RenderData.Instance.StructureGhost_World, 0);

                foreach (StructureData.GhostModels ghostModel in ghost.structureData.ghostModels)
                {
                    Graphics.DrawMesh(ghostModel.mesh, matrix, ghostModel.material, 0);
                }
            }
        }
    }

    public void DrawGhostAtMouse()
    {
        Material activeGhostMaterial;
        int2 ghostOrigin = GetMouseGhostPosition();
        bool blocked = false;

        //TODO Large structures should be blocked from being built when overlapping other large objects. This will require some thinking and some time. 

        blocked = IsGhostBlocked(ghostOrigin);

        if (blocked) activeGhostMaterial = RenderData.Instance.StructureGhost_Blocked;
        else activeGhostMaterial = RenderData.Instance.StructureGhost_Mouse;

        var matrix = MatrixConstruction.CreateTransformMatrix(ghostOrigin, _ghostRotation);

        Graphics.DrawMesh(GhostStructureData.ghostMesh, matrix, activeGhostMaterial, 0);

        foreach (StructureData.GhostModels ghostModel in GhostStructureData.ghostModels)
        {
            Material materialToDraw = ghostModel.material;
            if (ghostModel.material == GlobalData.Instance.mat_Ghost)
            {
                materialToDraw = activeGhostMaterial;
            }
            Graphics.DrawMesh(ghostModel.mesh, matrix, materialToDraw, 0);
        }
          
        foreach (TinyTransform input in GhostStructureData.inputs)
            DrawArrow(input, RenderData.Instance.Arrow, RenderData.Instance.ArrowOutputMaterial);  

        foreach (TinyTransform output in GhostStructureData.outputs) 
            DrawArrow(output, RenderData.Instance.Arrow, RenderData.Instance.ArrowInputMaterial);

        foreach (TinyTransform port in GhostStructureData.ports)
            DrawArrow(port, RenderData.Instance.TwoWayArrow, RenderData.Instance.UniversalMaterial);

        void DrawArrow(TinyTransform transform, Mesh mesh, Material material)
        {
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (transform.position.Rotate(_ghostRotation) + ghostOrigin).ToVector3() + (Vector3.up * 0.2f),
                transform.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(mesh, _matrix, material, 0);
        }

        if(GhostStructureData.name == "PowerPylon")
        {
            for (int x = -Electrical.Relay.defaultConnectionRange; x <= Electrical.Relay.defaultConnectionRange; x++)
                for (int y = -Electrical.Relay.defaultConnectionRange; y <= Electrical.Relay.defaultConnectionRange; y++)
                {
                    var _pos = new Vector3(x + ghostOrigin.x, 0.05f, y + ghostOrigin.y);
                    Graphics.DrawMesh(RenderData.Instance.TilePowerGizmo, _pos, Quaternion.identity, RenderData.Instance.TilePowerGizmoMaterial, 0);
                }
        }
    }


    static byte2 singleTileSize = new byte2(1, 1);
    bool IsGhostBlocked(int2 ghostOrigin)
    {
        if (GameManager.Instance.GameWorld.worldGrid.IsEntityAt(ghostOrigin)) return true;

        if (GameManager.Instance.GameWorld.floorGrid.IsEntityAt(ghostOrigin) && GameManager.Instance.GameWorld.floorGrid.GetEntityAt(ghostOrigin).GetType() == typeof(Crater)) return true;

        if (!GhostStructureData.size.Equals(singleTileSize))
        {
            var occupyLocations = Entity.GetOccupyingLocations(ghostOrigin, GhostStructureData.size, GhostRotation, GameManager.Instance.GameWorld.worldGrid);

            foreach(var location in occupyLocations) 
                if(location.entity != null) 
                    return true;  
        } 
        return false;
    }
} 
