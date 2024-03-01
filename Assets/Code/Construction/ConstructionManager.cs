
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionManager
{
    public StructureData newGhostData = new();

    public List<GhostStructure> ghosts = new List<GhostStructure>();

    [SerializeField] public Material ghostMaterial;

    byte _ghostRotation = 0;
    byte ghostRotation
    {
        get { return _ghostRotation; }
        set { _ghostRotation %= 4; }
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

        GameManager.Instance.gameWorld.worldGrid.AddEntity(newGhostStructure, position);
    }

    public void DrawGhosts()
    {
        if(ghosts.Count == 0) return;

        foreach (GhostStructure ghost in ghosts)
        {
            Graphics.DrawMesh(ghost.data.mesh, new Vector3(ghost.position.x, 0, ghost.position.y), quaternion.identity, ghost.data.material, 0);
        }
    }

    public void DrawGhostAtMouse(int2 position)
    {
        if (GameManager.Instance.gameWorld.worldGrid.IsEntityAt(position)) { Debug.Log("Failed to draw ghost"); return; }
            
        Graphics.DrawMesh(newGhostData.mesh, new Vector3(position.x, 0, position.y), quaternion.identity, newGhostData.material, 0);
    }

    public void RotateGhost(sbyte direction)
    {

    }
}

public class ConstructionTask
{

}
