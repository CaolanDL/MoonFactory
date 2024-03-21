 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods; 

public class ConstructionManager
{
    public StructureData newGhostData;

    public List<StructureGhost> ghosts = new List<StructureGhost>(); 

    [SerializeField] public Material ghostMaterial;

    [SerializeField] sbyte ghostRotation = 0;
    [SerializeField] sbyte GhostRotation
    {
        get { return ghostRotation; }
        set { ghostRotation = (sbyte)((value % 4 + 4) % 4); }
    } 

    public void StartPlacingGhosts(StructureData structureData)
    {
        ghostRotation = 0;
        newGhostData = structureData;
    }

    public void PlaceGhost(int2 position)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        StructureGhost newGhostStructure = new(newGhostData);

        if (worldGrid.TryAddEntity(newGhostStructure, position, ghostRotation) != null)
        {
            ghosts.Add(newGhostStructure);
            newGhostStructure.OnPlaced();
            newGhostStructure.FinishConstruction(); // Immediately finish building the structure on placement. Should be replaced with rover construction logic ASAP.
        }
    }

    public void RotateGhost(sbyte direction)
    {
        GhostRotation += direction;
    }


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
        if (ghosts.Count != 0)
        { 
            foreach (StructureGhost ghost in ghosts)
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

    public void DrawGhostAtMouse(int2 position)
    {
        Material activeGhostMaterial = GlobalData.Instance.mat_Ghost;

        if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(position))
        {
            activeGhostMaterial = GlobalData.Instance.mat_GhostBlocked;
        }

        var matrix = MatrixConstruction.CreateTransformMatrix(position, ghostRotation);

        Graphics.DrawMesh(newGhostData.ghostMesh, matrix, activeGhostMaterial, 0);

        foreach (StructureData.GhostModels ghostModel in newGhostData.ghostModels)
        {
            Material materialToDraw = ghostModel.material;
            if (ghostModel.material == GlobalData.Instance.mat_Ghost)
            {
                materialToDraw = activeGhostMaterial;
            }
            Graphics.DrawMesh(ghostModel.mesh, matrix, materialToDraw, 0);
        }
         
        // Draw indicator arrows ->
        foreach (TinyTransform input in newGhostData.inputs)
        {
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (input.position.Rotate(ghostRotation) + position).ToVector3() + (Vector3.up * 0.2f),
                input.rotation.Rotate(ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, _matrix, GlobalData.Instance.mat_ArrowIndicatorInput, 0);
        }

        foreach (TinyTransform output in newGhostData.outputs)
        { 
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (output.position.Rotate(ghostRotation) + position).ToVector3() + (Vector3.up * 0.2f),
                output.rotation.Rotate(ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, _matrix, GlobalData.Instance.mat_ArrowIndicatorOutput, 0);
        }
        // <- Draw indicator arrows
    }
} 
