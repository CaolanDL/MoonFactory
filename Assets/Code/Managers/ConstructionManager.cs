 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods; 

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

        if(GhostStructureData.name == "StaticDrill")
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillPlaced);
        }
    }

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

    /*    int2 GetOriginOffset(int2 origin, Vector2 centre, sbyte rotation)
        { 
        }*/

    /// <summary>
    /// Force a structure to spawn at a location. Development use only.
    /// </summary> 
    public void ForceSpawnStructure(int2 position, sbyte rotation, StructureData structureData)
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;

        worldGrid.RemoveEntity(position);

        Structure newStructure = StructureFactory.CreateStructure(structureData);

        worldGrid.TryAddEntity(newStructure, position, rotation);

        newStructure.Constructed();
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
        Material activeGhostMaterial = RenderData.Instance.StructureGhost_Mouse;

        int2 ghostGridPosition = GetMouseGhostPosition();

        bool blocked = false;

        //TODO Large structures should be blocked from being built when overlapping other large objects. This will require some thinking and some time. 

        if (GameManager.Instance.GameWorld.worldGrid.IsEntityAt(ghostGridPosition)) blocked = true; 

        if (blocked) activeGhostMaterial = RenderData.Instance.StructureGhost_Blocked; 

        var matrix = MatrixConstruction.CreateTransformMatrix(ghostGridPosition, _ghostRotation);

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
                (transform.position.Rotate(_ghostRotation) + ghostGridPosition).ToVector3() + (Vector3.up * 0.2f),
                transform.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(mesh, _matrix, material, 0);
        }

        if(GhostStructureData.name == "PowerPylon")
        {
            for (int x = -Electrical.Relay.connectionRange; x <= Electrical.Relay.connectionRange; x++)
                for (int y = -Electrical.Relay.connectionRange; y <= Electrical.Relay.connectionRange; y++)
                {
                    var _pos = new Vector3(x + ghostGridPosition.x, 0.05f, y + ghostGridPosition.y);
                    Graphics.DrawMesh(RenderData.Instance.TilePowerGizmo, _pos, Quaternion.identity, RenderData.Instance.TilePowerGizmoMaterial, 0);
                }
        }
    } 
} 
