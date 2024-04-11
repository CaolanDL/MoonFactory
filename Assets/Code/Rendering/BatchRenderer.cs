using System.Collections.Generic;
using UnityEngine;

public class BatchRenderer : MonoBehaviour
{
    WireRenderer wireRenderer = new();
    public GizmoRenderer gizmoRenderer = new();
    PlatformRenderer platformRenderer = new();

    public void Init()
    {
        wireRenderer.Init();
        platformRenderer.Init();
    }

    public void Render()
    {
        wireRenderer.RenderAll();
        gizmoRenderer.Tick();
        gizmoRenderer.Render();
        platformRenderer.RenderAll();
    } 

    public static void RenderChunkedMatrixArray(ChunkedMatrixArray chunkedMatrixArray, Mesh mesh, Material material)
    {
        for (int chunkIndex = 0; chunkIndex <= chunkedMatrixArray.matriceChunks.Count; chunkIndex++)
        {
            if (chunkIndex == chunkedMatrixArray.chunkIndex)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, chunkedMatrixArray.matriceChunks[chunkIndex], chunkedMatrixArray.itemIndex);
                break;
            }
            else
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, chunkedMatrixArray.matriceChunks[chunkIndex]);
            }
        }

        chunkedMatrixArray.Reset();
    }
}

public class WireRenderer
{
    ChunkedMatrixArray chunkedMatrixArray = new();
    public int RenderCount = 0;

    public Mesh mesh;
    public Material material;

    public void Init()
    {
        mesh = GameManager.Instance.RenderData.wire_mesh;
        material = GameManager.Instance.RenderData.wire_material;
    }

    public void RenderAll()
    {
        foreach(Electrical.Connection connection in Electrical.Connection.pool)
        {
            var matrix = Matrix4x4.TRS(connection.origin, connection.rotation, connection.scale);

            chunkedMatrixArray.QueueMatrix(matrix);
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, mesh, material);
    }

    public void RenderVisible()
    {

    }
}



public class GizmoRenderer
{
    List<Vector3> gizmoLocations = new();
    Dictionary<Vector3, int> gizmoLifespans = new();

    public void Add(Vector3 location, int lifespan)
    {
        if(gizmoLocations.Contains(location)) { return; }
        gizmoLocations.Add(location);
        gizmoLifespans.Add(location, lifespan);
    }

    public void Tick()
    {
        List<Vector3> gizmosToRemove = new();

        foreach(Vector3 location in gizmoLocations)
        {
            gizmoLifespans[location]--;
            if (gizmoLifespans[location] < 0) { gizmosToRemove.Add(location); }
        } 
        foreach(var item in gizmosToRemove)
        {
            gizmoLifespans.Remove(item);
            gizmoLocations.Remove(item);
        }
    } 

    public void Render()
    {
        foreach(var kvp in gizmoLifespans)
        {
            Graphics.DrawMesh(GlobalData.Instance.gizmo_Axis, kvp.Key, Quaternion.identity, GlobalData.Instance.mat_PulsingGizmo, 0);
        }
    } 
}