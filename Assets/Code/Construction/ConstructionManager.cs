 
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
        set { ghostRotation = (sbyte)(value % 4); }
    }

    int2 dragPlacementOrigin = new();

    public void DrawGhosts()
    {
        if (ghosts.Count != 0)
        { 
            foreach (StructureGhost ghost in ghosts)
            {
                var matrix = MatrixConstruction.CreateTransformMatrix(ghost.position, ghost.rotation);

                Graphics.DrawMesh(ghost.data.ghostMesh, matrix, GlobalData.Instance.mat_Ghost, 0);

                foreach (StructureData.GhostModels ghostModel in ghost.data.ghostModels)
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
            if(ghostModel.material == GlobalData.Instance.mat_Ghost)
            {
                materialToDraw = activeGhostMaterial; 
            }
            Graphics.DrawMesh(ghostModel.mesh, matrix, materialToDraw, 0);
        }


        Vector3 _position = new Vector3(position.x, 0, position.y);

        Quaternion _rotation = Quaternion.Euler(0, 90 * GhostRotation, 0);

        // Draw indicator arrows
        foreach (var arrow in newGhostData.arrowIndicators)
        {
            Quaternion arrowRotation = (arrow.rotation * _rotation).normalized;

            Vector3 arrowPosition = (_rotation * arrow.relativePosition) + _position;

            Matrix4x4 _matrix = Matrix4x4.TRS
            (
                arrowPosition,
                arrowRotation,
                Vector3.one * arrow.size
            ); 

            Graphics.DrawMesh(GlobalData.Instance.m_ArrowIndicator, _matrix, GlobalData.Instance.mat_ArrowIndicator, 0);
        }
    }

    public void StartPlacingGhosts(StructureData structureData)
    {
        ghostRotation = 0;
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
