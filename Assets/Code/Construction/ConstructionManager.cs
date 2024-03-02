
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ConstructionManager
{
    public StructureData newGhostData;

    public List<GhostStructure> ghosts = new List<GhostStructure>();

    [SerializeField] public Material ghostMaterial;

    [SerializeField] sbyte ghostRotation = 0;
    [SerializeField] sbyte GhostRotation
    {
        get { return ghostRotation; }
        set { ghostRotation = (sbyte)(value % 4); }
    }

    public void DrawGhosts()
    {
        if (ghosts.Count == 0) return;

        foreach (GhostStructure ghost in ghosts)
        {
            Graphics.DrawMesh(ghost.data.mesh, new Vector3(ghost.position.x, 0, ghost.position.y), Quaternion.Euler(0, 90 * ghost.rotation, 0), GlobalData.Instance.mat_ghost, 0);
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
            var arrowPosition = (arrow.rotation *  arrow.relativePosition) + _position;

            var arrowRotation = arrow.rotation * _rotation;

            Matrix4x4 matrix = Matrix4x4.TRS
            (
                (arrowRotation * arrow.relativePosition) + _position,
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
        if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(position)) return;

        GhostStructure newGhostStructure = new(newGhostData);

        ghosts.Add(newGhostStructure);

        newGhostStructure.position = position;

        newGhostStructure.rotation = ghostRotation;

        GameManager.Instance.gameWorld.worldGrid.AddEntity(newGhostStructure, position);
    }  

    public void RotateGhost(sbyte direction)
    { 
        GhostRotation += direction;
    }
}

public class ConstructionTask
{

}
