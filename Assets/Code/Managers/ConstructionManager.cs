 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods; 

public class ConstructionManager
{
    public StructureData NewGhostData;

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
        NewGhostData = structureData;
    }

    public void PlaceGhost(double2 mousePosition)
    {
        var ghostGridPosition = GetMouseGhostPosition();

        var worldGrid = GameManager.Instance.GameWorld.worldGrid;

        StructureGhost newGhostStructure = new(NewGhostData);

        if (worldGrid.TryAddEntity(newGhostStructure, ghostGridPosition, _ghostRotation) != null)
        {
            Ghosts.Add(newGhostStructure);
            newGhostStructure.OnPlaced();

            if(DevFlags.InstantBuilding)
            {
                newGhostStructure.FinishConstruction();
            } 
        }
    }

    public void RotateGhost(sbyte direction)
    {
        GhostRotation += direction;
    } 

    public int2 GetMouseGhostPosition()
    {
        var inputManager = GameManager.Instance.PlayerInputManager;

        if (NewGhostData.size.x > 1 && NewGhostData.size.y > 1)
        { 
            var n = (-NewGhostData.centre).Rotate(GhostRotation * 90);

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
        var rSize = new int2(NewGhostData.size.x, NewGhostData.size.y).Rotate(GhostRotation);
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

                Graphics.DrawMesh(ghost.structureData.ghostMesh, matrix, GlobalData.Instance.mat_Ghost, 0);

                foreach (StructureData.GhostModels ghostModel in ghost.structureData.ghostModels)
                {
                    Graphics.DrawMesh(ghostModel.mesh, matrix, ghostModel.material, 0);
                }
            }
        }
    }

    public void DrawGhostAtMouse()
    {
        Material activeGhostMaterial = GlobalData.Instance.mat_Ghost;

        int2 ghostGridPosition = GetMouseGhostPosition();

        bool blocked = false;

        //TODO Large structures should be blocked from being built when overlapping other large objects. This will require some thinking and some time.
        //? You really need to take a little holiday. You are starting to feel the burn-out.

        if (GameManager.Instance.GameWorld.worldGrid.IsEntityAt(ghostGridPosition))
        {
            blocked = true;
        }


        if (blocked)
        {
            activeGhostMaterial = GlobalData.Instance.mat_GhostBlocked;
        }


        var matrix = MatrixConstruction.CreateTransformMatrix(ghostGridPosition, _ghostRotation);

        Graphics.DrawMesh(NewGhostData.ghostMesh, matrix, activeGhostMaterial, 0);

        foreach (StructureData.GhostModels ghostModel in NewGhostData.ghostModels)
        {
            Material materialToDraw = ghostModel.material;
            if (ghostModel.material == GlobalData.Instance.mat_Ghost)
            {
                materialToDraw = activeGhostMaterial;
            }
            Graphics.DrawMesh(ghostModel.mesh, matrix, materialToDraw, 0);
        }
         
        // Draw indicator arrows ->
        foreach (TinyTransform input in NewGhostData.inputs)
            DrawArrow(input, RenderData.Instance.Arrow, RenderData.Instance.ArrowOutputMaterial);  

        foreach (TinyTransform output in NewGhostData.outputs) 
            DrawArrow(output, RenderData.Instance.Arrow, RenderData.Instance.ArrowInputMaterial);

        foreach (TinyTransform port in NewGhostData.ports)
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
        // <- Draw indicator arrows
    }
} 
