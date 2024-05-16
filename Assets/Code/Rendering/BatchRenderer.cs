using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BatchRenderer : MonoBehaviour
{
    WireRenderer wireRenderer = new();
    public GizmoRenderer gizmoRenderer = new();
    BridgeRenderer platformRenderer = new();
    public ItemRenderer ItemRenderer { get { return GameManager.Instance.ItemRenderer; } }
    ElectricalCoverageRenderer electricalCoverageRenderer = new();

    public void Init()
    {
        wireRenderer.Init();
        platformRenderer.Init();
    }

    public void Tick()
    {
        gizmoRenderer.Tick(); 
    }

    public void Render()
    {
        wireRenderer.RenderAll();
        gizmoRenderer.Render();
        platformRenderer.RenderAll();

        electricalCoverageRenderer.TryRender();
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
            if (connection.scale.magnitude < 0.1f) { continue; }
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


public class ElectricalCoverageRenderer
{
    List<Electrical.Relay> Relays { get { return Electrical.SystemManager.relays; } }
    ChunkedMatrixArray chunkedMatrixArray = new();

    public static bool enabled = false;

    public void TryRender()
    {
        if (enabled == false) return;

        List<Vector3> coveredPositions = new();

        foreach(var relay in Relays)
        {
            for (int x = -Electrical.Relay.connectionRange-1; x <= Electrical.Relay.connectionRange+1; x++)
            {
                for (int y = -Electrical.Relay.connectionRange-1; y <= Electrical.Relay.connectionRange+1; y++)
                { 
                    var pos = new Vector3(x + relay.Parent.position.x, 0.05f, y + relay.Parent.position.y);
                    if (coveredPositions.Contains(pos)) { continue; }
                    coveredPositions.Add(pos);
                    var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one); 
                    chunkedMatrixArray.QueueMatrix(matrix); 
                }
            }
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, RenderData.Instance.TilePowerGizmo, RenderData.Instance.TilePowerGizmoMaterial);
    }
}