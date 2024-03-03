
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ConstructionManager
{
    public StructureData newGhostData;

    public List<StructureGhost> ghosts = new List<StructureGhost>(); 

    [SerializeField] public Material ghostMaterial;

    [SerializeField] sbyte ghostRotation = 0;
    [SerializeField] sbyte GhostRotation
    {
        get { return ghostRotation; }
        set { ghostRotation = (sbyte)(value % 4); }
    }

    public void DrawGhosts()
    {
        if (ghosts.Count != 0)
        { 
            foreach (StructureGhost ghost in ghosts)
            {
                var matrix = CreateTransformMatrix(ghost.position, ghost.rotation);

                Graphics.DrawMesh(ghost.data.mesh, matrix, GlobalData.Instance.mat_ghost, 0);
            }
        }
        // Development Only > Replace with animation gameobjects
        foreach (Structure structure in Structure.structures)
        {
            var matrix = CreateTransformMatrix(structure.position, structure.rotation); 

            Graphics.DrawMesh(structure.data.mesh, matrix, GlobalData.Instance.mat_DevUniversal, 0);

            foreach(StructureData.AdditiveModelData additiveModel in structure.data.additiveModels)
            {
                Graphics.DrawMesh(additiveModel.mesh, matrix, additiveModel.material, 0); 
            }
        }

        Matrix4x4 CreateTransformMatrix(int2 location, sbyte rotation)
        {
            return Matrix4x4.TRS
            (
                new Vector3(location.x, 0, location.y),
                Quaternion.Euler(0, 90 * rotation, 0),
                Vector3.one
            );
        }
    }

    public void DrawGhostAtMouse(int2 position)
    {
        Material ghostMaterial = GlobalData.Instance.mat_ghost;

        if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(position))
        {
            ghostMaterial = GlobalData.Instance.mat_ghostBlocked;
        }

        Quaternion _rotation = Quaternion.Euler(0, 90 * GhostRotation, 0);

        Vector3 _position = new Vector3(position.x, 0, position.y);

        Graphics.DrawMesh(newGhostData.mesh, _position, _rotation, ghostMaterial, 0);

        // Draw indicator arrows
        foreach(var arrow in newGhostData.arrowIndicators)
        {
            Quaternion arrowRotation = (arrow.rotation * _rotation).normalized;

            Vector3 arrowPosition = (_rotation * arrow.relativePosition) + _position;

            Matrix4x4 matrix = Matrix4x4.TRS
            (
                arrowPosition,
                arrowRotation,
                Vector3.one * arrow.size
            ); 

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, matrix, GlobalData.Instance.mat_ArrowIndicator, 0);
        }
    }

    public void StartPlacingGhosts(StructureData structureData)
    {
        newGhostData = structureData;
    }

    public void PlaceGhost(int2 position)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        if (worldGrid.IsEntityAt(position)) return;

        StructureGhost newGhostStructure = new(newGhostData);

        ghosts.Add(newGhostStructure); 

        worldGrid.AddEntity(newGhostStructure, position, ghostRotation);

        newGhostStructure.FinishConstruction(); // Immediately finish building the structure on placement. Should be replaced with rover construction logic ASAP.
    }  

    public void RotateGhost(sbyte direction)
    { 
        GhostRotation += direction;
    }
} 
