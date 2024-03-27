 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.UIElements;

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

        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        StructureGhost newGhostStructure = new(NewGhostData);

        if (worldGrid.TryAddEntity(newGhostStructure, ghostGridPosition, _ghostRotation) != null)
        {
            Ghosts.Add(newGhostStructure);
            newGhostStructure.OnPlaced();
            //newGhostStructure.FinishConstruction(); // Immediately finish building the structure on placement. Should be replaced with rover construction logic ASAP.
        }
    }

    public void RotateGhost(sbyte direction)
    {
        GhostRotation += direction;
    } 

    public int2 GetMouseGhostPosition()
    {
        var inputManager = GameManager.Instance.playerInputManager;

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
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

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

        if (NewGhostData.size.x > 1 && NewGhostData.size.y > 1)
        {
            (int2 xRange, int2 yRange) occupyRegion = GetOccupyRegion(ghostGridPosition); 

            if (GameManager.Instance.gameWorld.worldGrid.IsEntityInArea(occupyRegion.xRange, occupyRegion.yRange)) blocked = true;  
        }
        else
        { 
            if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(ghostGridPosition))
            {
                blocked = true;
            }
        } 


        if(blocked)
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
        {
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (input.position.Rotate(_ghostRotation) + ghostGridPosition).ToVector3() + (Vector3.up * 0.2f),
                input.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.gizmo_Arrow, _matrix, GlobalData.Instance.mat_ArrowIndicatorInput, 0);
        }

        foreach (TinyTransform output in NewGhostData.outputs)
        { 
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (output.position.Rotate(_ghostRotation) + ghostGridPosition).ToVector3() + (Vector3.up * 0.2f),
                output.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.gizmo_Arrow, _matrix, GlobalData.Instance.mat_ArrowIndicatorOutput, 0);
        }
        // <- Draw indicator arrows
    }
} 
