 
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

    public void PlaceGhost(int2 position)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        StructureGhost newGhostStructure = new(NewGhostData);

        if (worldGrid.TryAddEntity(newGhostStructure, position, _ghostRotation) != null)
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

    public void DrawGhostAtMouse(int2 position)
    {
        Material activeGhostMaterial = GlobalData.Instance.mat_Ghost;

        if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(position))
        {
            activeGhostMaterial = GlobalData.Instance.mat_GhostBlocked;
        }

        var matrix = MatrixConstruction.CreateTransformMatrix(position, _ghostRotation);

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
                (input.position.Rotate(_ghostRotation) + position).ToVector3() + (Vector3.up * 0.2f),
                input.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, _matrix, GlobalData.Instance.mat_ArrowIndicatorInput, 0);
        }

        foreach (TinyTransform output in NewGhostData.outputs)
        { 
            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                (output.position.Rotate(_ghostRotation) + position).ToVector3() + (Vector3.up * 0.2f),
                output.rotation.Rotate(_ghostRotation).ToQuaternion(),
                Vector3.one * 0.2f
            );

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, _matrix, GlobalData.Instance.mat_ArrowIndicatorOutput, 0);
        }
        // <- Draw indicator arrows
    }
} 
